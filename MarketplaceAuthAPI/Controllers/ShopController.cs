using BLL.Model.Constants;
using BLL.Model.DTO;
using BLL.Model.ResponseModel;
using BLL.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceAuthAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = IdentityRoles.Shop)]
public class ShopController : Controller
{
    private readonly IShopService _shopService;

    public ShopController(IShopService shopService)
    {
        _shopService = shopService;
    }
    
    [HttpGet("GetUserInfo")]
    public async Task<IActionResult> GetUserInfo(int userId, int? addressId)
    { 
        var res = new ServiceResponse<UserShopView>();

        if (addressId != null && addressId > 0)
        {
            res = await _shopService.GetUserInfoById(userId, (int)addressId);
        }
        else
        {
            res = await _shopService.GetUserInfoById(userId);
        }

        if (res.IsSuccess)
        {
            return Ok(res);
        }
        return BadRequest(res);
    }
}