using System.ComponentModel.DataAnnotations;
using BLL.Model.Attribute;

namespace BLL.Model.RequestModel;

public class ChangePasswordModel
{
    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [NotEqual("OldPassword")]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; }
}