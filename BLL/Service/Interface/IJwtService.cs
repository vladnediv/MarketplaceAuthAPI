using Domain.Model;

namespace BLL.Service.Interface;

public interface IJwtService
{
    public Task<string> GenerateAccessTokenAsync(ApplicationUser user);
    public Task<string> GenerateRefreshTokenAsync();
}