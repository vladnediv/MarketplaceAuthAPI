using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel.HelperModel;

public class RegisterMarketplaceShop : IRegisterModel
{
    [Required]
    public string ShopName { get; set; }
    [Required]
    public string LogoUrl { get; set; }
    [Required]
    public AddressDTO Address { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
}