using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel;

public class LoginUserModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}