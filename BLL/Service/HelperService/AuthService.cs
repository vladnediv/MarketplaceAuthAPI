using System.Security.Claims;
using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel;
using BLL.Model.ServiceResponse;
using BLL.Service.Interface;
using Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IJwtService _jwtService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }
    
    public async Task<ServiceResponse<IdentityError>> ChangePassword(ApplicationUser currentUser, ChangePasswordModel changePasswordModel)
    {
        var res = await _userManager.ChangePasswordAsync(currentUser, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
        ServiceResponse<IdentityError> serviceRes = new ServiceResponse<IdentityError>();
        if (res.Succeeded)
        {
            serviceRes.IsSuccess = true;
        }
        else
        {
            serviceRes.IsSuccess = false;
            serviceRes.Entities = res.Errors.ToList();
        }
        return serviceRes;
    }

    public async Task<ServiceResponse<TokenModel>> RefreshTokenAsync(string refreshToken)
    {
        ServiceResponse<ApplicationUser> res = await GetApplicationUserByRefreshToken(refreshToken);
        
        ServiceResponse<TokenModel> serviceRes = new ServiceResponse<TokenModel>();
        
        if (res.IsSuccess)
        {
            if (res.Entity.RefreshToken != refreshToken || res.Entity.RefreshTokenExpireTime < DateTime.UtcNow)
            {
                serviceRes.IsSuccess = false;
                serviceRes.Message = ServiceResponseMessages.InvalidRefreshToken;
            }
            else
            {
                string newAccessToken = await _jwtService.GenerateAccessTokenAsync(res.Entity);
                string newRefreshToken = await _jwtService.GenerateRefreshTokenAsync();
                
                res.Entity.RefreshToken = newRefreshToken;
                res.Entity.RefreshTokenExpireTime = DateTime.UtcNow.AddMinutes(2);
                
                IdentityResult identityResult = await _userManager.UpdateAsync(res.Entity);
                if (!identityResult.Succeeded)
                {
                    serviceRes.IsSuccess = false;
                    serviceRes.Message = identityResult.Errors.First().Description;
                }
                else
                {
                    
                    serviceRes.IsSuccess = true;
                    serviceRes.Entity = new TokenModel();
                    serviceRes.Entity.RefreshToken = newRefreshToken;
                    serviceRes.Entity.AccessToken = newAccessToken;
                    serviceRes.Entity.Role = GetUserRole(res.Entity);
                }
            }
        }
        else
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = ServiceResponseMessages.UserNotFound;
        }
        return serviceRes;
    }

    public string GetUserRole(ApplicationUser user)
    {
        string role = "";
        role = user.MarketplaceUserId.HasValue ? IdentityRoles.User : 
            user.MarketplaceShopId.HasValue ? IdentityRoles.Shop :
            user.MarketplaceAdminId.HasValue ? IdentityRoles.Admin :
            IdentityRoles.User;
        
        return role;
    }
    public async Task<ServiceResponse<ApplicationUser>> GetApplicationUserByLoginAsync(string login)
    {
        ApplicationUser? user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == login);
        
        ServiceResponse<ApplicationUser> serviceRes = new ServiceResponse<ApplicationUser>();

        if (user == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = ServiceResponseMessages.UserNotFound;
        }
        else
        {
            serviceRes.IsSuccess = true;
            serviceRes.Entity = user;
        }
        return serviceRes;
    }
    public async Task<ServiceResponse<ApplicationUser>> GetApplicationUserByRefreshToken(string token)
    {
        ApplicationUser? user = _userManager.Users.FirstOrDefault(x => x.RefreshToken == token);
        
        ServiceResponse<ApplicationUser> serviceRes = new ServiceResponse<ApplicationUser>();
        
        if (user == null)
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = ServiceResponseMessages.UserNotFound;
        }
        else
        {
            serviceRes.IsSuccess = true;
            serviceRes.Entity = user;
        }
        return serviceRes;
    }

    public async Task<ServiceResponse<IdentityError>> DeleteApplicationUserByIdAsync(int id)
    {
        ApplicationUser? user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        
        ServiceResponse<IdentityError> serviceRes = new ServiceResponse<IdentityError>();
        if (user != null)
        {
            var identityRes = await _userManager.DeleteAsync(user);

            if (identityRes.Succeeded)
            {
                serviceRes.IsSuccess = true;
            }
            else
            {
                serviceRes.IsSuccess = false;
                serviceRes.Message = identityRes.Errors.First().Description;
            }
        }
        else
        {
            serviceRes.IsSuccess = false;
            serviceRes.Message = ServiceResponseMessages.UserNotFound;
        }
        return serviceRes;
    }
    public async Task<ServiceResponse<IdentityError>> AddToRoleAsync(ApplicationUser user, string roleName)
    {
        bool roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            IdentityResult res = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            if (!res.Succeeded)
            {
                return new ServiceResponse<IdentityError>()
                {
                    IsSuccess = false,
                    Entities = res.Errors.ToList()
                };
            }
        }

        
        IdentityResult addRes = await _userManager.AddToRoleAsync(user, roleName);
        if (addRes.Succeeded)
        {
            return new ServiceResponse<IdentityError>()
            {
                IsSuccess = true
            };
        }
        else
        {
            return new ServiceResponse<IdentityError>()
            {
                IsSuccess = false,
                Entities = addRes.Errors.ToList()
            };
        }
    }

    public int GetUserIdFromClaims(ClaimsPrincipal user)
    {
        return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}