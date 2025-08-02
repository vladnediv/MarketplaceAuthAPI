using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BLL.Model.RequestModel.HelperModel;

public class RegisterMarketplaceShop : IRegisterModel
{
    [Required]
    public string ShopName { get; set; }
    public string? LogoUrl { get; set; }
    [ValidateNever]
    public List<AddressDTO> Addresses { get; set; }
    
    public string PhoneNumber { get; set; }
}