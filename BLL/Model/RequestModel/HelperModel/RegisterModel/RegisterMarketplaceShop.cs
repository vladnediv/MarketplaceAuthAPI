using System.ComponentModel.DataAnnotations;
using BLL.Model.DTO;
using BLL.Model.RequestModel.HelperModel.Interface;

namespace BLL.Model.RequestModel.HelperModel.RegisterModel;

public class RegisterMarketplaceShop : IRegisterModel
{
    [Required]
    public string ShopName { get; set; }
    public string? LogoUrl { get; set; }
    public List<AddressDTO>? Addresses { get; set; }
    
    public string PhoneNumber { get; set; }
}