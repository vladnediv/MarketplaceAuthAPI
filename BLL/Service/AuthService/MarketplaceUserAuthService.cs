using System.Text.RegularExpressions;
using BLL.Model.Constants;
using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel.RegisterModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ResponseModel;
using BLL.Service.Interface;
using Domain.Model;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service.AuthService;

public class MarketplaceUserAuthService : HelperService.AuthService, IUserAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IGenericService<MarketplaceUser> _userService;
    private readonly ISmsService _smsService;
    private readonly IOtpService _otpService;
    
    public MarketplaceUserAuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IJwtService jwtService,
        IGenericService<MarketplaceUser> userService,
        ISmsService smsService,
        IOtpService otpService
        ) : base(userManager, roleManager, jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _userService = userService;
        _smsService = smsService;
        _otpService = otpService;
    }
    
    public async Task<ServiceResponse<IdentityError>> RegisterAsync(GenericRegisterUserModel<RegisterMarketplaceUser> genericRegisterUserModel)
    {
        var serviceRes = new ServiceResponse<IdentityError>();

        var applicationUser = new ApplicationUser
        {
            PhoneNumber = genericRegisterUserModel.PhoneNumber,
            Email = genericRegisterUserModel.Email,
            UserName = genericRegisterUserModel.Email
        };

        var marketplaceUser = new MarketplaceUser
        {
            FirstName = genericRegisterUserModel.UserModel.FirstName,
            LastName = genericRegisterUserModel.UserModel.LastName
        };

        IdentityResult createRes = await _userManager.CreateAsync(applicationUser, genericRegisterUserModel.Password);
        if (!createRes.Succeeded)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Entities = createRes.Errors.ToList();
            serviceRes.Message = createRes.Errors.FirstOrDefault().Description;
            return serviceRes;
        }

        var user = await _userManager.FindByEmailAsync(genericRegisterUserModel.Email);
        if (user == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = ServiceResponseMessages.CreateFailed + " " + ServiceResponseMessages.UnexpectedError;
            return serviceRes;
        }

        

        var roleRes = await AddToRoleAsync(user, IdentityRoles.User);
        if (!roleRes.IsSuccess)
        {
            await _userManager.DeleteAsync(user);
            serviceRes.IsSuccess = false;
            serviceRes.Entities = roleRes.Entities;
            serviceRes.Message = roleRes.Message;
            return serviceRes;
        }

        var relationRes = await ConfigureRelationAsync(marketplaceUser, user.Email);
        if (!relationRes.IsSuccess)
        {
            await _userManager.DeleteAsync(user);
            serviceRes.IsSuccess = false;
            serviceRes.Message = relationRes.Message;
            return serviceRes;
        }

        serviceRes.IsSuccess = true;
        return serviceRes;
    }
    
    private async Task<ServiceResponse> ConfigureRelationAsync(MarketplaceUser marketplaceUser, string email)
    {
        ServiceResponse serviceRes = new ServiceResponse();

        // Get the ApplicationUser
        var applicationUserRes = await GetApplicationUserByLoginAsync(email);

        if (!applicationUserRes.IsSuccess || applicationUserRes.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = applicationUserRes.Message;
            return serviceRes;
        }

        // Assign the ApplicationUserId to the MarketplaceUser
        marketplaceUser.ApplicationUserId = applicationUserRes.Entity.Id;

        // Create the MarketplaceUser
        var marketplaceUserRes = await _userService.CreateAsync(marketplaceUser);

        if (!marketplaceUserRes.IsSuccess || marketplaceUserRes.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = marketplaceUserRes.Message;
            return serviceRes;
        }

        // Update the ApplicationUser with reference to MarketplaceUser
        applicationUserRes.Entity.MarketplaceUserId = marketplaceUserRes.Entity.Id;

        var updateRes = await _userManager.UpdateAsync(applicationUserRes.Entity);
        if (!updateRes.Succeeded)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = updateRes.Errors.FirstOrDefault().Description;
            return serviceRes;
        }
        
        serviceRes.IsSuccess = true;
        return serviceRes;
    }
    
    public async Task<ServiceResponse<TokenModel>> LoginAsync(LoginUserModel loginUserModel)
    {
        //check if user logs in via email or phone number
        ApplicationUser? user = new ApplicationUser();
        bool byEmail = false;
        if (loginUserModel.Email != null && loginUserModel.Email.Length > 0)
        {
            byEmail = true;
           user = await _userManager.FindByEmailAsync(loginUserModel.Email);
        }
        else if(loginUserModel.Phone != null && loginUserModel.Phone.Length > 0)
        {
            byEmail = false;
            user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == loginUserModel.Phone);
        }
        else
        {
            return new ServiceResponse<TokenModel>()
            {
                IsSuccess = false,
                Message = ServiceResponseMessages.ArgumentsAreNull
            };
        }
        ServiceResponse<TokenModel> serviceRes = new ServiceResponse<TokenModel>();
        
        if (user != null)
        {
            if (!user.MarketplaceUserId.HasValue)
            {
                serviceRes.IsSuccess = false;
                serviceRes.Message = ServiceResponseMessages.UnexpectedError;
                return serviceRes;
            }

            //by email -> validate password
            if (byEmail)
            {
                var isValid = await _userManager.CheckPasswordAsync(user, loginUserModel.Password);
                if (isValid)
                {
                    user.RefreshToken = await _jwtService.GenerateRefreshTokenAsync();
                    user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
                    var managerRes = await _userManager.UpdateAsync(user);

                    if (managerRes.Succeeded)
                    {
                        serviceRes.IsSuccess = true;
                        var accessToken = await _jwtService.GenerateAccessTokenAsync(user);
                        serviceRes.Entity = new TokenModel();
                        serviceRes.Entity.AccessToken = accessToken;
                        serviceRes.Entity.RefreshToken = user.RefreshToken;
                        serviceRes.Entity.Role = IdentityRoles.User;
                    
                        return serviceRes;
                    }

                    serviceRes.IsSuccess = false;
                
                    return serviceRes;
                }
                else
                {
                    serviceRes.IsSuccess = false;
                    serviceRes.Message = ServiceResponseMessages.InvalidPassword;
                
                    return serviceRes;
                }
            }
            //by phone number -> not allowed, need to call method "VerifyOtp"
            else
            {
               serviceRes.IsSuccess = false;
               serviceRes.Message = ServiceResponseMessages.UnexpectedError;
               return serviceRes;
            }
        }
        serviceRes.Message = ServiceResponseMessages.InvalidLogin;
        
        return serviceRes;
    }

    public async Task<ServiceResponse<IdentityError>> UpdateUserAsync(UpdateUserModel<UpdateMarketplaceUser> model,  int userId)
    {
        var entity = await _userService.GetAsync(userId);
        
        var serviceRes = new ServiceResponse<IdentityError>();
        
        if (entity.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = entity.Message;
            return serviceRes;
        }
        
        entity.Entity.FirstName = model.User.FirstName;
        entity.Entity.LastName = model.User.LastName;
        
        var res = await _userService.UpdateAsync(entity.Entity);

        if (!res.IsSuccess)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = res.Message;
            return serviceRes;
        }
        
        serviceRes.IsSuccess = true;
        return serviceRes;
    }
    
    public async Task<ServiceResponse> DeleteUserAsync(int marketplaceUserId)
    {
        ServiceResponse serviceRes = new ServiceResponse();
        
        var userRes = await _userService.GetAsync(marketplaceUserId);

        if (!userRes.IsSuccess)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = userRes.Message;
            
            return serviceRes;
        }

        var deleteRes = await DeleteApplicationUserByIdAsync(userRes.Entity.ApplicationUserId);

        if (!deleteRes.IsSuccess)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = deleteRes.Message;
            
            return serviceRes;
        }
        
        serviceRes.IsSuccess = true;
        return serviceRes;
    }

    
    
    /// <summary>
    /// This method checks whether a login is phone number or email.
    /// </summary>
    /// <param name="login"></param>
    /// <returns>Returns true if a login is valid. If login is email -> Message = Email, if login is phone number -> Message = PhoneNumber.</returns>
    public async Task<ServiceResponse> CheckLoginAsync(string login)
    {
        var res = new ServiceResponse();

        //if login is a phone number, search for a user with this phone number
        if (IsPhoneNumber(login))
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == login);
            if (user == null)
            {
                res.IsSuccess = false;
                res.Message = ServiceResponseMessages.UserNotFound;
                
                return res;
            }

            var sendOtpRes = await _smsService.SendOTPAsync(login);
            if (!sendOtpRes.IsSuccess)
            {
                res.IsSuccess = false;
                res.Message = sendOtpRes.Message;
                
                return res;
            }
            
            res.IsSuccess = true;
            res.Message = "PhoneNumber";
            
            return res;
        }
        else
        {
            //if login is email, search for a user with this email
            var user = await _userManager.FindByEmailAsync(login);
            if (user == null)
            {
                res.IsSuccess = false;
                res.Message = ServiceResponseMessages.UserNotFound;
                
                return res;
            }
            res.IsSuccess = true;
            res.Message = "Email";
            return res;
        }
    }

    public async Task<ServiceResponse<TokenModel>> GoogleAuthAsync(GoogleLoginModel model)
    {
        var res = new ServiceResponse<TokenModel>();

        var payload = await GetPayloadFromIdTokenAsync(model.IdToken);

        DateTimeOffset expireTime = payload != null? DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds) : DateTimeOffset.Now;
        if (payload != null && DateTimeOffset.UtcNow < expireTime)
        {
            
            var findRes = await _userManager.FindByEmailAsync(payload.Email);
            if (findRes != null)
            {
                //login
                var jwt = await _jwtService.GenerateAccessTokenAsync(findRes);
                var refreshToken = await _jwtService.GenerateRefreshTokenAsync();
                
                res.IsSuccess = true;
                res.Entity = new TokenModel();
                res.Entity.RefreshToken = refreshToken;
                res.Entity.AccessToken = jwt;
                
                return res;
            }
            else
            {
                //register
                GenericRegisterUserModel<RegisterMarketplaceUser> registerModel =
                    new GenericRegisterUserModel<RegisterMarketplaceUser>()
                    {
                        Email = payload.Email,
                        //TODO Send email with this temporary password
                        Password = Guid.NewGuid().ToString(),
                        PhoneNumber = "",
                        UserModel = new RegisterMarketplaceUser()
                        {
                            FirstName = payload.GivenName,
                            LastName = payload.FamilyName
                        }
                    };
                
                var registerRes = await RegisterAsync(registerModel);
                if (registerRes.IsSuccess)
                {
                    res.IsSuccess = true;
                    res.Message = "Registered";
                    
                    return res;
                }
                else
                {
                    res.IsSuccess = false;
                    res.Message = registerRes.Message;
                    
                    return res;
                }
            }
        }
        else
        {
            res.IsSuccess = false;
            res.Message = ServiceResponseMessages.UnexpectedError;
        }

        return res;
    }

    public async Task<ServiceResponse> VerifyOtpAsync(string login, string otp)
    {
        var res = _otpService.VerifyOtp(login, otp);

        return new  ServiceResponse()
        {
            IsSuccess = res,
            Message = res? "" : "Invalid code!"
        };
    }

    private async Task<GoogleJsonWebSignature.Payload> GetPayloadFromIdTokenAsync(string token)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(token, new GoogleJsonWebSignature.ValidationSettings
        {
            //here we should read the audience from some key vault
            Audience = new[] { "580941447228-5ljmuricq42jr02kpo87gl5lpfqhk8se.apps.googleusercontent.com" }
        });
        
        return payload;
    }

    private bool IsPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;
        
        string phonePattern = @"^\+?[\d\s\-()]{7,20}$";
        return Regex.IsMatch(phoneNumber, phonePattern);
    }
}