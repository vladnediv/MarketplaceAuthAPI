using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel.HelperModel;

public class RegisterMarketplaceUser : IRegisterModel
{
    public string FirstName  { get; set; }
    public string LastName  { get; set; }
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
}