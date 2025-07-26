using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ServiceResponse;
using BLL.Service.Interface;
using Domain.Model;
using Microsoft.AspNetCore.Identity;

namespace BLL.Service;

public class MarketplaceShopAuthService : AuthService, IUserAuthService<RegisterMarketplaceShop, UpdateMarketplaceShop>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IJwtService _jwtService;
    
    public MarketplaceShopAuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IJwtService jwtService
    ) : base(userManager, roleManager, jwtService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }
    
    public async Task<ServiceResponse<IdentityError>> RegisterAsync(RegisterUserModel<RegisterMarketplaceShop> registerUserModel)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<TokenModel>> LoginAsync(LoginUserModel loginUserModel)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<IdentityError>> UpdateUserAsync(UpdateUserModel<UpdateMarketplaceShop> model,  int userId)
    {
        throw new NotImplementedException();
    }
}