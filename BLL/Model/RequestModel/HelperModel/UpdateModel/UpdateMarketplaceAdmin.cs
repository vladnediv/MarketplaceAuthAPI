using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel.HelperModel.UpdateModel;

public class UpdateMarketplaceAdmin : IUpdateUser
{
    [Required]
    public string Name { get; set; }
}