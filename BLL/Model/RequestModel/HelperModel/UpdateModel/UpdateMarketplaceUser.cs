using System.ComponentModel.DataAnnotations;
using BLL.Model.RequestModel.HelperModel.Interface;

namespace BLL.Model.RequestModel.HelperModel.UpdateModel;

public class UpdateMarketplaceUser : IUpdateUser
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}