using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ServiceResponse;
using BLL.Service.Interface;
using Domain.Model;
using Microsoft.AspNetCore.Identity;

namespace BLL.Service;

public class MarketplaceUserAuthService : AuthService, IUserAuthService<RegisterMarketplaceUser, UpdateMarketplaceUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IGenericService<MarketplaceUser> _userService;
    
    public MarketplaceUserAuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IJwtService jwtService,
        IGenericService<MarketplaceUser> userService
        ) : base(userManager, roleManager, jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _userService = userService;
    }
    
    public async Task<ServiceResponse<IdentityError>> RegisterAsync(RegisterUserModel<RegisterMarketplaceUser> registerUserModel)
    {
        var serviceRes = new ServiceResponse<IdentityError>();

        var applicationUser = new ApplicationUser
        {
            PhoneNumber = registerUserModel.UserModel.PhoneNumber,
            Email = registerUserModel.Email,
            UserName = registerUserModel.Email
        };

        var marketplaceUser = new MarketplaceUser
        {
            FirstName = registerUserModel.UserModel.FirstName,
            LastName = registerUserModel.UserModel.LastName
        };

        IdentityResult createRes = await _userManager.CreateAsync(applicationUser, registerUserModel.Password);
        if (!createRes.Succeeded)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Entities = createRes.Errors.ToList();
            return serviceRes;
        }

        var user = await _userManager.FindByEmailAsync(registerUserModel.Email);
        if (user == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = "User could not be found after creation.";
            return serviceRes;
        }

        

        var roleRes = await AddToRoleAsync(user, IdentityRoles.User);
        if (!roleRes.IsSuccess)
        {
            await _userManager.DeleteAsync(user);
            serviceRes.IsSuccess = false;
            serviceRes.Entities = roleRes.Entities;
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
            serviceRes.Message = applicationUserRes.Message ?? "Application user not found.";
            return serviceRes;
        }

        // Assign the ApplicationUserId to the MarketplaceUser
        marketplaceUser.ApplicationUserId = applicationUserRes.Entity.Id;

        // Create the MarketplaceUser
        var marketplaceUserRes = await _userService.CreateAsync(marketplaceUser);

        if (!marketplaceUserRes.IsSuccess || marketplaceUserRes.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = marketplaceUserRes.Message ?? "Failed to create MarketplaceUser.";
            return serviceRes;
        }

        // Update the ApplicationUser with reference to MarketplaceUser
        applicationUserRes.Entity.MarketplaceUserId = marketplaceUserRes.Entity.Id;

        var updateRes = await _userManager.UpdateAsync(applicationUserRes.Entity);
        if (!updateRes.Succeeded)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = updateRes.Errors.FirstOrDefault()?.Description ?? "Failed to update ApplicationUser.";
            return serviceRes;
        }
        
        serviceRes.IsSuccess = true;
        return serviceRes;
    }
    
    public async Task<ServiceResponse<TokenModel>> LoginAsync(LoginUserModel loginUserModel)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(loginUserModel.Email);
        
        ServiceResponse<TokenModel> serviceRes = new ServiceResponse<TokenModel>();
        
        if (user != null)
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
                serviceRes.Message = "Invalid username or password.";
                
                return serviceRes;
            }
        }
        serviceRes.IsSuccess = false;
        serviceRes.Message = "Invalid username or password.";
        
        return serviceRes;
    }

    public async Task<ServiceResponse<IdentityError>> UpdateUserAsync(UpdateUserModel<UpdateMarketplaceUser> model,  int userId)
    {
        var entity = await _userService.GetAsync(userId);
        
        var serviceRes = new ServiceResponse<IdentityError>();
        
        if (entity.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = "User could not be found.";
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
}