using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel;

public class LoginUserModel
{
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}   