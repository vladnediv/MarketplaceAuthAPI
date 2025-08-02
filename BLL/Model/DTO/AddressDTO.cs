using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel.HelperModel;

public class AddressDTO
{
    [Required]
    public string StreetName { get; set; }
    [Required]
    public string StreetNumber { get; set; }
    [Required]
    public string FloorNumber { get; set; }
    [Required]
    public string PostalCode { get; set; }
    [Required]
    public string CityName { get; set; }
    [Required]
    public string CountryName { get; set; }
}