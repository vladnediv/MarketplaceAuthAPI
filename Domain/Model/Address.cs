namespace Domain.Model;

public class Address
{
    public int Id { get; set; }
    public string Street { get; set; }
    public string CityName { get; set; }
    public int? UserId { get; set; }
    public int? ShopId { get; set; }
    public MarketplaceUser? User { get; set; }
    public MarketplaceShop? Shop { get; set; }
}