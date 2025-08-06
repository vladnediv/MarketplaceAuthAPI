using System.Security.Claims;
using BLL.Model.RequestModel;
using BLL.Model.RequestModel.HelperModel;
using BLL.Model.RequestModel.HelperModel.UpdateModel;
using BLL.Model.ServiceResponse;
using BLL.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceAuthAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = IdentityRoles.User)]
public class UserController : Controller
{
    private readonly MarketplaceUserAuthService _userAuthService;

    public UserController(MarketplaceUserAuthService userAuthService)
    {
        _userAuthService = userAuthService;
    }

    [HttpPost("Update")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserModel<UpdateMarketplaceUser> model)
    {
        var email = User.FindFirst(ClaimTypes.Name)?.Value;
        var res = await _userAuthService.GetApplicationUserByLoginAsync(email);
        if (!res.IsSuccess)
        {
            return BadRequest(res);
        }

        var result = await _userAuthService.UpdateUserAsync(model, (int)res.Entity.MarketplaceUserId);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("Delete")]
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