using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel.Interface;


namespace BLL.Model.RequestModel;

public class UpdateUserModel<T> where T : IUpdateUser
{
    public int Id { get; set; }
    [Required]
    public T User { get; set; }
}