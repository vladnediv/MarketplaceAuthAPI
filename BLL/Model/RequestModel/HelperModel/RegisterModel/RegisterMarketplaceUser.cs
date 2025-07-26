using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel.HelperModel;

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