using AutoMapper;
using BLL.Model.Constants;
using BLL.Model.DTO;
using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel.RegisterModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ResponseModel;
using BLL.Service.Interface;
using Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service.AuthService;

public class MarketplaceAdminAuthService : HelperService.AuthService, IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IGenericService<MarketplaceAdmin> _adminService;
    private readonly IMapper _mapper;

    public MarketplaceAdminAuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IJwtService jwtService,
        IGenericService<MarketplaceAdmin> adminService,
        IMapper mapper
    ) : base(userManager, roleManager, jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _adminService = adminService;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IdentityError>> RegisterAsync(GenericRegisterUserModel<RegisterMarketplaceAdmin> genericRegisterUserModel)
    {
        var serviceRes = new ServiceResponse<IdentityError>();

        var applicationUser = new ApplicationUser
        {
            Email = genericRegisterUserModel.Email,
            UserName = genericRegisterUserModel.Email
        };

        var marketplaceAdmin = new MarketplaceAdmin
        {
            Name = genericRegisterUserModel.UserModel.Name
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

        var roleRes = await AddToRoleAsync(user, IdentityRoles.Admin);
        if (!roleRes.IsSuccess)
        {
            await _userManager.DeleteAsync(user);
            serviceRes.IsSuccess = false;
            serviceRes.Entities = roleRes.Entities;
            serviceRes.Message = roleRes.Message;
            return serviceRes;
        }

        var relationRes = await ConfigureRelationAsync(marketplaceAdmin, user.Email);
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

    private async Task<ServiceResponse> ConfigureRelationAsync(MarketplaceAdmin admin, string email)
    {
        ServiceResponse serviceRes = new ServiceResponse();

        var applicationUserRes = await GetApplicationUserByLoginAsync(email);

        if (!applicationUserRes.IsSuccess || applicationUserRes.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = applicationUserRes.Message;
            return serviceRes;
        }

        admin.ApplicationUserId = applicationUserRes.Entity.Id;

        var adminRes = await _adminService.CreateAsync(admin);
        if (!adminRes.IsSuccess || adminRes.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = adminRes.Message;
            return serviceRes;
        }

        applicationUserRes.Entity.MarketplaceAdminId = adminRes.Entity.Id;

        var updateRes = await _userManager.UpdateAsync(applicationUserRes.Entity);
        if (!updateRes.Succeeded)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = updateRes.Errors.FirstOrDefault()?.Description;
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
            
            if (!user.MarketplaceAdminId.HasValue)
            {
                serviceRes.IsSuccess = false;
                serviceRes.Message = ServiceResponseMessages.UnexpectedError;
                return serviceRes;
            }
            
            var isValid = await _userManager.CheckPasswordAsync(user, loginUserModel.Password);
            if (isValid)
            {
                var isSuperAdmin = await _userManager.IsInRoleAsync(user, IdentityRoles.SuperAdmin);
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
                        Role = isSuperAdmin ? IdentityRoles.SuperAdmin : IdentityRoles.Admin
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

    public async Task<ServiceResponse<IdentityError>> UpdateUserAsync(UpdateUserModel<UpdateMarketplaceAdmin> model, int userId)
    {
        var entity = await _adminService.GetAsync(userId);
        var serviceRes = new ServiceResponse<IdentityError>();

        if (entity.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = entity.Message;
            return serviceRes;
        }

        entity.Entity.Name = model.User.Name;

        var res = await _adminService.UpdateAsync(entity.Entity);
        if (!res.IsSuccess)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = res.Message;
            return serviceRes;
        }

        serviceRes.IsSuccess = true;
        return serviceRes;
    }

    public async Task<ServiceResponse> DeleteUserAsync(int marketplaceAdminId)
    {
        ServiceResponse serviceRes = new ServiceResponse();
        
        var adminRes = await _adminService.GetAsync(marketplaceAdminId);

        if (!adminRes.IsSuccess)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = adminRes.Message;
            
            return serviceRes;
        }

        var deleteRes = await DeleteApplicationUserByIdAsync(adminRes.Entity.ApplicationUserId);

        if (!deleteRes.IsSuccess)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = deleteRes.Message;
            
            return serviceRes;
        }
        
        serviceRes.IsSuccess = true;
        return serviceRes;
    }

    public async Task<ServiceResponse<MarketplaceShopDTO>> GetShopByIdAsync(int shopId)
    {
        ServiceResponse<MarketplaceShopDTO> serviceRes = new ServiceResponse<MarketplaceShopDTO>();
        
        var shop = await _userManager.Users.FirstOrDefaultAsync(x => x.MarketplaceShopId == shopId);
        if (shop == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = ServiceResponseMessages.UserNotFoundById(shopId);
        }
        else
        {
            serviceRes.IsSuccess = true;
            serviceRes.Entity = _mapper.Map<MarketplaceShopDTO>(shop.MarketplaceShop);
        }
        return serviceRes;
    }

    public async Task<ServiceResponse<MarketplaceShopDTO>> GetShopsAsync()
    {
        ServiceResponse<MarketplaceShopDTO> serviceRes = new ServiceResponse<MarketplaceShopDTO>();
        
        var shops = await _userManager.Users.Where(x => x.MarketplaceShopId != null).ToListAsync();

        if (shops == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = ServiceResponseMessages.UnexpectedError;
        }
        else
        {
            serviceRes.IsSuccess = true;
            
        }

        return new  ServiceResponse<MarketplaceShopDTO>();
    }

    public async Task<ServiceResponse<MarketplaceUserDTO>> GetUserAsync(int userId)
    {
        throw new NotImplementedException();
    }
}