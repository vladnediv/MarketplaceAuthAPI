using BLL.Model.Constants;
using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel.RegisterModel;
using BLL.Model.ResponseModel;
using BLL.Service;
using BLL.Service.AuthService;
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

    public AuthController(MarketplaceUserAuthService userAuthService, MarketplaceShopAuthService shopAuthService,
        MarketplaceAdminAuthService adminAuthService)
    {
        _userAuthService = userAuthService;
        _shopAuthService = shopAuthService;
        _adminAuthService = adminAuthService;
    }

    [HttpGet("TestConnection")]
    public async Task<IActionResult> TestConnection()
    {
        return Ok("Connection successful");
    }

    [HttpPost("RegisterUser")]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserModel<RegisterMarketplaceUser> model)
    {
        ServiceResponse<IdentityError> result = await _userAuthService.RegisterAsync(model.RegisterModel);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("RegisterShop")]
    public async Task<IActionResult> RegisterShopAsync([FromBody] RegisterUserModel<RegisterMarketplaceShop> model)
    {
        ServiceResponse<IdentityError> result = await _shopAuthService.RegisterAsync(model.RegisterModel);

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
                userResult.Message == ServiceResponseMessages.InvalidPassword ||
                userResult.Message == ServiceResponseMessages.ArgumentsAreNull)
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

    [HttpPost("CheckLogin")]
    public async Task<IActionResult> CheckLoginAsync([FromBody] string login)
    {
        var result = await _userAuthService.CheckLoginAsync(login);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
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

    [HttpPost("Logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        var res = await _userAuthService.LogoutUserByClaimsAsync(User);

        if (res.IsSuccess)
        {
            return Ok(res);
        }
        return BadRequest(res);
    }

}