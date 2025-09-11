namespace BLL.Model.DTO;

public class MarketplaceShopDTO
{
    public string Name { get; set; }
    public List<AddressDTO>? Addresses { get; set; }
    public string Email { get; set; }
}