using System.Security.Claims;
using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Service;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost("RegisterUser")]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserModel<RegisterMarketplaceUser> model)
    {
        var result = await _userAuthService.RegisterAsync(model);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("RegisterShop")]
    public async Task<IActionResult> RegisterShopAsync([FromBody] RegisterUserModel<RegisterMarketplaceShop> model)
    {
        var result = await _shopAuthService.RegisterAsync(model);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("RegisterAdmin")]
    public async Task<IActionResult> RegisterAdminAsync([FromBody] RegisterUserModel<RegisterMarketplaceAdmin> model)
    {
        var result = await _adminAuthService.RegisterAsync(model);
        if (result.IsSuccess)
            return Ok(result);
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