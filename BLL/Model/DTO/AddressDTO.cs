using System.ComponentModel.DataAnnotations;

namespace BLL.Model.RequestModel.HelperModel;

public class AddressDTO
{
    public string? StreetName { get; set; }

    public string? StreetNumber { get; set; }

    public string? FloorNumber { get; set; }

    public string? PostalCode { get; set; }

    public string? CityName { get; set; }

    public string? CountryName { get; set; }
}