using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel;
using BLL.Model.ServiceResponse;
using BLL.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceAuthAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly MarketplaceUserAuthService _userAuthService;
    private readonly MarketplaceShopAuthService _shopAuthService;
    private readonly MarketplaceAdminAuthService _adminAuthService;

    public AuthController(MarketplaceUserAuthService userAuthService,  MarketplaceShopAuthService shopAuthService, MarketplaceAdminAuthService adminAuthService)
    {
        _userAuthService = userAuthService;
        _shopAuthService = shopAuthService;
        _adminAuthService = adminAuthService;
    }
    
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserModel<RegisterMarketplaceUser, RegisterMarketplaceShop> model)
    {
        ServiceResponse<IdentityError> result;
        if (model.IsUser)
        {
            result = await _userAuthService.RegisterAsync(model.User);
        }
        else
        {
            result = await _shopAuthService.RegisterAsync(model.Shop);
        }

        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserModel model)
    {
        var userResult = await _userAuthService.LoginAsync(model);
        if (!userResult.IsSuccess)
        {
            var shopResult = await _shopAuthService.LoginAsync(model);
            if (!shopResult.IsSuccess)
            {
                var adminResult = await _adminAuthService.LoginAsync(model);
                if (!adminResult.IsSuccess)
                {
                    return Unauthorized();
                }
                return Ok(adminResult);
            }
            return Ok(shopResult);
        }
        return Ok(userResult);
    }
    
    
}