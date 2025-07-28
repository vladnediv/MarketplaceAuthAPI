using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel;

namespace BLL.Model.RequestModel;

public class GenericRegisterUserModel<T> where T : IRegisterModel
{
    [Required]
    public T UserModel { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}