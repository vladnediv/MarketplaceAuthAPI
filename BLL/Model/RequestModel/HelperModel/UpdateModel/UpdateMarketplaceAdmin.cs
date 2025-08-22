using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel.Interface;

namespace BLL.Model.RequestModel.HelperModel.UpdateModel;

public class UpdateMarketplaceAdmin : IUpdateUser
{
    [Required]
    public string Name { get; set; }
}