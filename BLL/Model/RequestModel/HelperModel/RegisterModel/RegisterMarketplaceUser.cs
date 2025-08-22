using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel.Interface;

namespace BLL.Model.RequestModel.HelperModel.RegisterModel;

public class RegisterMarketplaceUser : IRegisterModel
{
    [Required]
    public string FirstName  { get; set; }
    [Required]
    public string LastName  { get; set; }
    [Required]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
}