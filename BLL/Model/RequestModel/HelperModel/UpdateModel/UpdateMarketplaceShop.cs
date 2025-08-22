using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel.Interface;

namespace BLL.Model.RequestModel.HelperModel.UpdateModel;

public class UpdateMarketplaceShop : IUpdateUser
{
    [Required]
    public string LogoUrl { get; set; }
    [Required]
    public string Name { get; set; }
}