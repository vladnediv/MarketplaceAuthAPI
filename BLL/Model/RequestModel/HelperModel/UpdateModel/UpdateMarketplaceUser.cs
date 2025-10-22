using System.ComponentModel.DataAnnotations;
using BLL.Model.DTO;
using BLL.Model.RequestModel.HelperModel.Interface;
using Microsoft.AspNetCore.Http;

namespace BLL.Model.RequestModel.HelperModel.UpdateModel;

public class UpdateMarketplaceUser : IUpdateUser
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.PhoneNumber)]
    [Phone]
    public string Phone { get; set; }
    
    [Required]
    public AddressDTO Address { get; set; }
    
    public string? PictureUrl { get; set; }
    public IFormFile? Picture { get; set; }
}