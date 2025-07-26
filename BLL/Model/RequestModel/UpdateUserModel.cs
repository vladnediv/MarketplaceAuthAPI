using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel;


namespace BLL.Model.RequestModel;

public class UpdateUserModel<T> where T : IUpdateUser
{
    [Required]
    public T User { get; set; }
}