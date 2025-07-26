using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ServiceResponse;
using BLL.Service.Interface;
using Domain.Model;
using Microsoft.AspNetCore.Identity;

namespace BLL.Service;

public class MarketplaceAdminAuthService : AuthService, IUserAuthService<RegisterMarketplaceAdmin, UpdateMarketplaceAdmin>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IGenericService<MarketplaceAdmin> _adminService;

    public MarketplaceAdminAuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IJwtService jwtService,
        IGenericService<MarketplaceAdmin> adminService
    ) : base(userManager, roleManager, jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _adminService = adminService;
    }

    public async Task<ServiceResponse<IdentityError>> RegisterAsync(RegisterUserModel<RegisterMarketplaceAdmin> registerUserModel)
    {
        var serviceRes = new ServiceResponse<IdentityError>();

        var applicationUser = new ApplicationUser
        {
            Email = registerUserModel.Email,
            UserName = registerUserModel.Email
        };

        var marketplaceAdmin = new MarketplaceAdmin
        {
            Name = registerUserModel.UserModel.Name
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

        var roleRes = await AddToRoleAsync(user, IdentityRoles.Admin);
        if (!roleRes.IsSuccess)
        {
            await _userManager.DeleteAsync(user);
            serviceRes.IsSuccess = false;
            serviceRes.Entities = roleRes.Entities;
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
            serviceRes.Message = applicationUserRes.Message ?? "Application user not found.";
            return serviceRes;
        }

        admin.ApplicationUserId = applicationUserRes.Entity.Id;

        var adminRes = await _adminService.CreateAsync(admin);
        if (!adminRes.IsSuccess || adminRes.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = adminRes.Message ?? "Failed to create MarketplaceAdmin.";
            return serviceRes;
        }

        applicationUserRes.Entity.MarketplaceAdminId = adminRes.Entity.Id;

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
        var user = await _userManager.FindByEmailAsync(loginUserModel.Email);
        var serviceRes = new ServiceResponse<TokenModel>();

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
                    var accessToken = await _jwtService.GenerateAccessTokenAsync(user);

                    serviceRes.IsSuccess = true;
                    serviceRes.Entity = new TokenModel
                    {
                        AccessToken = accessToken,
                        RefreshToken = user.RefreshToken,
                        Role = IdentityRoles.Admin
                    };

                    return serviceRes;
                }

                serviceRes.IsSuccess = false;
                return serviceRes;
            }

            serviceRes.IsSuccess = false;
            serviceRes.Message = "Invalid username or password.";
            return serviceRes;
        }

        serviceRes.IsSuccess = false;
        serviceRes.Message = "Invalid username or password.";
        return serviceRes;
    }

    public async Task<ServiceResponse<IdentityError>> UpdateUserAsync(UpdateUserModel<UpdateMarketplaceAdmin> model, int userId)
    {
        var entity = await _adminService.GetAsync(userId);
        var serviceRes = new ServiceResponse<IdentityError>();

        if (entity.Entity == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = "Admin could not be found.";
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
}