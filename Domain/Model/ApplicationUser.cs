using Microsoft.AspNetCore.Identity;

namespace Domain.Model;

public class ApplicationUser : IdentityUser
{
    public int? MarketplaceUserId { get; set; }
    public int? MarketplaceShopId { get; set; }
    public int? MarketplaceAdminId { get; set; }

    public MarketplaceUser? MarketplaceUser { get; set; }
    public MarketplaceShop? MarketplaceShop { get; set; }
    public MarketplaceAdmin? MarketplaceAdmin { get; set; }
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpireTime { get; set; }
}