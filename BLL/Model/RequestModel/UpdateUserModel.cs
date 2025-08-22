using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel.Interface;


namespace BLL.Model.RequestModel;

public class UpdateUserModel<T> where T : IUpdateUser
{
    [Required]
    public T User { get; set; }
}