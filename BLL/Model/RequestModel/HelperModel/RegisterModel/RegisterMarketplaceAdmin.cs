using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel.HelperModel;

public class RegisterMarketplaceAdmin : IRegisterModel
{
    [Required]
    public string Name { get; set; }
}