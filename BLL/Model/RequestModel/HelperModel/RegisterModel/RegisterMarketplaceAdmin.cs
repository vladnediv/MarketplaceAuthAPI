using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel.Interface;

namespace BLL.Model.RequestModel.HelperModel.RegisterModel;

public class RegisterMarketplaceAdmin : IRegisterModel
{
    [Required]
    public string Name { get; set; }
}