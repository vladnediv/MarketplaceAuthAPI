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

            if (userResult.Message == ServiceResponseMessages.InvalidLogin ||
                userResult.Message == ServiceResponseMessages.InvalidPassword)
            {
                return Unauthorized(userResult);
            }
            
            var shopResult = await _shopAuthService.LoginAsync(model);
            if (!shopResult.IsSuccess)
            {
                
                if (shopResult.Message == ServiceResponseMessages.InvalidLogin ||
                    shopResult.Message == ServiceResponseMessages.InvalidPassword)
                {
                    return Unauthorized(shopResult);
                }
                
                var adminResult = await _adminAuthService.LoginAsync(model);
                if (!adminResult.IsSuccess)
                {
                    return Unauthorized(adminResult);
                }
                return Ok(adminResult);
            }
            return Ok(shopResult);
        }
        return Ok(userResult);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshTokenAsync(string refreshToken)
    {
        var res = await _userAuthService.RefreshTokenAsync(refreshToken);
        if (res.IsSuccess)
        {
            return Ok(res);
        }
        return BadRequest(res);
    }
}   