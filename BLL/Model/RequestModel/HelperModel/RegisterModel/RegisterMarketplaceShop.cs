using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel.HelperModel;

public class RegisterMarketplaceShop : IRegisterModel
{
    [Required]
    public string ShopName { get; set; }

    public string? LogoUrl { get; set; }
    
    public List<AddressDTO>? Addresses { get; set; }
    
    [Required]
    public string PhoneNumber { get; set; }
}