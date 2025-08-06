using BLL.Model.RequestModel.HelperModel;
using BLL.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceAuthAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = IdentityRoles.Admin)]
public class AdminController : Controller
{
     private readonly MarketplaceAdminAuthService  _adminService;

     public AdminController(MarketplaceAdminAuthService adminService)
     {
          _adminService = adminService;
     }
     
     
}