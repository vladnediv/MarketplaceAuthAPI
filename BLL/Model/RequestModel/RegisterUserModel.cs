using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel;

namespace BLL.Model.RequestModel;

public class RegisterUserModel<TUser, TShop>
where TUser : IRegisterModel
where TShop : IRegisterModel
{
    public GenericRegisterUserModel<TUser>? User { get; set; }
    public GenericRegisterUserModel<TShop>? Shop { get; set; }
    
    [Required]
    public bool IsUser { get; set; }
}