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
public class UserController : Controller
{
    private readonly MarketplaceUserAuthService _authService;

    public UserController(MarketplaceUserAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("Update")]
    [Authorize(Roles = IdentityRoles.User)]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserModel<UpdateMarketplaceUser> model)
    {
        var email = User.FindFirst(ClaimTypes.Name)?.Value;
        var res = await _authService.GetApplicationUserByLoginAsync(email);
        if (!res.IsSuccess)
        {
            return BadRequest(res);
        } 
        var result = await _authService.UpdateUserAsync(model, (int)res.Entity.MarketplaceUserId);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }
}