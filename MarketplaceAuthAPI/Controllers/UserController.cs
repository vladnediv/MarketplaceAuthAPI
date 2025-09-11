using System.Security.Claims;
using BLL.Model.Constants;
using BLL.Model.DTO;
using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ResponseModel;
using BLL.Service;
using BLL.Service.AuthService;
using BLL.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;

namespace MarketplaceAuthAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = IdentityRoles.User)]
public class UserController : Controller
{
    private readonly MarketplaceUserAuthService _userAuthService;
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _env;

    public UserController(
        MarketplaceUserAuthService userAuthService,
        IUserService userService,
        IWebHostEnvironment env)
    {
        _userAuthService = userAuthService;
        _userService = userService;
        _env = env;
    }

    [HttpGet("GetPersonalInfo")]
    public async Task<IActionResult> GetPersonalInfoAsync()
    {
        var res = new ServiceResponse<MarketplaceUserDTO>();
        
        //get user Id from jwt token claims
        var userId = _userAuthService.GetUserIdFromClaims(User);
        if (userId == 0)
        {
            res.IsSuccess = false;
            res.Message = ServiceResponseMessages.UserNotFound;
            
            return Unauthorized(res);
        }
        var getRes = await _userService.GetPersonalInfoAsync(userId);

        if (getRes.IsSuccess)
        {
            res.IsSuccess = true;
            res.Entity = getRes.Entity;

            return Ok(res);
        }
        return BadRequest(res);
    }
    
    [HttpPost("UpdatePersonalInfo")]
    public async Task<IActionResult> UpdateAsync([FromForm] UpdateUserModel<UpdateMarketplaceUser> model)
    {
        /*
        var email = User.FindFirst(ClaimTypes.Name)?.Value;
        var res = await _userAuthService.GetApplicationUserByLoginAsync(email);
        if (!res.IsSuccess)
        {
            return BadRequest(res);
        }
        */

        var result = await _userService.EditPersonalInfoAsync(model, _env.WebRootPath);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("DeleteAccount")]
    public async Task<IActionResult> DeleteAsync()
    {
        ServiceResponse res = new ServiceResponse();
        int userId = _userAuthService.GetUserIdFromClaims(User);

        if (userId == 0 || userId == null)
        {
            res.IsSuccess = false;
            res.Message = ServiceResponseMessages.UnexpectedError;
            return BadRequest(res);
        }

        res = await _userAuthService.DeleteUserAsync(userId);
        
        if (res.IsSuccess)
            return Ok(res);
        return BadRequest(res);
    }
}