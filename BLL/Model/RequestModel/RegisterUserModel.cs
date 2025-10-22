using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel.Interface;
using BLL.Model.RequestModel.HelperModel.RegisterModel;

namespace BLL.Model.RequestModel;

public class RegisterUserModel<TRegisterModel>
where TRegisterModel : IRegisterModel
{
    [Required]
    public GenericRegisterUserModel<TRegisterModel> RegisterModel { get; set; }
}