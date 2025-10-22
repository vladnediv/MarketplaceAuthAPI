using System.Security.Claims;
using BLL.Model.RequestModel;
using BLL.Model.ResponseModel;
using Domain.Model;
using Microsoft.AspNetCore.Identity;

namespace BLL.Service.Interface;

public interface IAuthService
{
    public Task<ServiceResponse<IdentityError>> ChangePassword(ApplicationUser currentUser, ChangePasswordModel changePasswordModel);
    public Task<ServiceResponse<TokenModel>> RefreshTokenAsync(string refreshToken);
    public Task<ServiceResponse<ApplicationUser>> GetApplicationUserByLoginAsync(string login);
    public Task<ServiceResponse<ApplicationUser>> GetApplicationUserByRefreshToken(string token);
    public Task<ServiceResponse<IdentityError>> DeleteApplicationUserByIdAsync(int id);
    public Task<ServiceResponse<IdentityError>> AddToRoleAsync(ApplicationUser user, string roleName);
    public int GetUserIdFromClaims(ClaimsPrincipal user);
}