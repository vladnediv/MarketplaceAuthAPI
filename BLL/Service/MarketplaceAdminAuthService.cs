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
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IJwtService _jwtService;
    
    public MarketplaceAdminAuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IJwtService jwtService
    ) : base(userManager, roleManager, jwtService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }

    public async Task<ServiceResponse<IdentityError>> RegisterAsync(RegisterUserModel<RegisterMarketplaceAdmin> registerUserModel)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<TokenModel>> LoginAsync(LoginUserModel loginUserModel)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IdentityError>> UpdateUserAsync(UpdateUserModel<UpdateMarketplaceAdmin> model, int userId)
    {
        throw new NotImplementedException();
    }
}