namespace Domain.Model;

public class Address
{
    public int Id { get; set; }
    public string StreetName  { get; set; }
    public string StreetNumber { get; set; }
    public string FloorNumber { get; set; }
    public string PostalCode { get; set; }
    public string CityName { get; set; }
    public string CountryName { get; set; }
    public int? UserId { get; set; }
    public int? ShopId { get; set; }
    public MarketplaceUser? User { get; set; }
    public MarketplaceShop? Shop { get; set; }
}