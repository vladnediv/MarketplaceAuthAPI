using BLL.Model.Constants;
using BLL.Service;
using BLL.Service.AuthService;
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