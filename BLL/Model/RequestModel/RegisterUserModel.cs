using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel;

namespace BLL.Model.RequestModel;

public class RegisterUserModel<TRegisterModel>
where TRegisterModel : IRegisterModel
{
    [Required]
    public GenericRegisterUserModel<TRegisterModel> RegisterModel { get; set; }
}