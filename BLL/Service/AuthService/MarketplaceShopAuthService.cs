using AutoMapper;
using BLL.Model.Constants;
using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel.RegisterModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ResponseModel;
using BLL.Service.Interface;
using Domain.Model;
using Microsoft.AspNetCore.Identity;

namespace BLL.Service.AuthService;

public class MarketplaceShopAuthService : HelperService.AuthService, IGenericUserAuthService<RegisterMarketplaceShop, UpdateMarketplaceShop>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IGenericService<MarketplaceShop> _shopService;
    private readonly IMapper _mapper;

    public MarketplaceShopAuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IJwtService jwtService,
        IGenericService<MarketplaceShop> shopService,
        IMapper mapper
    ) : base(userManager, roleManager, jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _shopService = shopService;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IdentityError>> RegisterAsync(GenericRegisterUserModel<RegisterMarketplaceShop> genericRegisterUserModel)
    {
        var serviceRes = new ServiceResponse<IdentityError>();

        var applicationUser = new ApplicationUser
        {
            PhoneNumber = genericRegisterUserModel.PhoneNumber,
            Email = genericRegisterUserModel.Email,
            UserName = genericRegisterUserModel.Email
        };

        var marketplaceShop = new MarketplaceShop
        {
            Name = genericRegisterUserModel.UserModel.ShopName,
        };
        if (genericRegisterUserModel.UserModel.Addresses.FirstOrDefault().CityName != "" )
        {
            marketplaceShop.Addresses = genericRegisterUserModel.UserModel.Addresses
                .Select(x => _mapper.Map<Address>(x)).ToList();
        }

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

        var roleRes = await AddToRoleAsync(user, IdentityRoles.Shop);
        if (!roleRes.IsSuccess)
        {
            await _userManager.DeleteAsync(user);
            serviceRes.IsSuccess = false;
            serviceRes.Entities = roleRes.Entities;
            serviceRes.Message = roleRes.Message;
            return serviceRes;
        }

        var relationRes = await ConfigureRelationAsync(marketplaceShop, user.Email);
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

    private async Task<ServiceResponse> ConfigureRelationAsync(MarketplaceShop marketplaceShop, string email)
    {
        ServiceResponse serviceRes = new ServiceResponse();

        var applicationUserRes = await GetApplicationUserByLoginAsync(email);

        if (!applicationUserRes.IsSuccess || applicationUserRes.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = applicationUserRes.Message;
            return serviceRes;
        }

        marketplaceShop.ApplicationUserId = applicationUserRes.Entity.Id;

        var marketplaceShopRes = await _shopService.CreateAsync(marketplaceShop);

        if (!marketplaceShopRes.IsSuccess || marketplaceShopRes.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = marketplaceShopRes.Message;
            return serviceRes;
        }

        applicationUserRes.Entity.MarketplaceShopId = marketplaceShopRes.Entity.Id;

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
        var user = await _userManager.FindByEmailAsync(loginUserModel.Email);
        var serviceRes = new ServiceResponse<TokenModel>();

        if (user != null)
        {
            
            if (!user.MarketplaceShopId.HasValue)
            {
                serviceRes.IsSuccess = false;
                serviceRes.Message = ServiceResponseMessages.UnexpectedError;
                return serviceRes;
            }
            
            var isValid = await _userManager.CheckPasswordAsync(user, loginUserModel.Password);
            if (isValid)
            {
                user.RefreshToken = await _jwtService.GenerateRefreshTokenAsync();
                user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
                var managerRes = await _userManager.UpdateAsync(user);

                if (managerRes.Succeeded)
                {
                    var accessToken = await _jwtService.GenerateAccessTokenAsync(user);

                    serviceRes.IsSuccess = true;
                    serviceRes.Entity = new TokenModel
                    {
                        AccessToken = accessToken,
                        RefreshToken = user.RefreshToken,
                        Role = IdentityRoles.Shop
                    };

                    return serviceRes;
                }

                serviceRes.IsSuccess = false;
                serviceRes.Message = managerRes.Errors.FirstOrDefault().Description;
                return serviceRes;
            }

            serviceRes.IsSuccess = false;
            serviceRes.Message = ServiceResponseMessages.InvalidPassword;
            return serviceRes;
        }

        serviceRes.IsSuccess = false;
        serviceRes.Message = ServiceResponseMessages.InvalidLogin;
        return serviceRes;
    }

    public async Task<ServiceResponse<IdentityError>> UpdateUserAsync(UpdateUserModel<UpdateMarketplaceShop> model, int userId)
    {
        var entity = await _shopService.GetAsync(userId);
        var serviceRes = new ServiceResponse<IdentityError>();

        if (entity.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = ServiceResponseMessages.UserNotFoundById(userId);
            return serviceRes;
        }

        entity.Entity.Name = model.User.Name;
        entity.Entity.LogoUrl = model.User.LogoUrl;

        var res = await _shopService.UpdateAsync(entity.Entity);
        if (!res.IsSuccess)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = res.Message;
            return serviceRes;
        }

        serviceRes.IsSuccess = true;
        return serviceRes;
    }
    
    public async Task<ServiceResponse> DeleteUserAsync(int marketplaceShopId)
    {
        ServiceResponse serviceRes = new ServiceResponse();
        
        var shopRes = await _shopService.GetAsync(marketplaceShopId);

        if (!shopRes.IsSuccess)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = shopRes.Message;
            
            return serviceRes;
        }

        var deleteRes = await DeleteApplicationUserByIdAsync(shopRes.Entity.ApplicationUserId);

        if (!deleteRes.IsSuccess)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = deleteRes.Message;
            
            return serviceRes;
        }
        
        serviceRes.IsSuccess = true;
        return serviceRes;
    }
}