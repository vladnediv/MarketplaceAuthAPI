using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel.HelperModel.UpdateModel;

public class UpdateMarketplaceUser : IUpdateUser
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}