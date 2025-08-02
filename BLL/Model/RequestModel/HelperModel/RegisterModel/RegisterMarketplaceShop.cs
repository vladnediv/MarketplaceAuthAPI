using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel.HelperModel;

public class RegisterMarketplaceShop : IRegisterModel
{
    public string ShopName { get; set; }

    public string? LogoUrl { get; set; }
    
    public List<AddressDTO>? Addresses { get; set; }
    
    public string PhoneNumber { get; set; }
}