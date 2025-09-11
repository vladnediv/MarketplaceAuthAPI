namespace BLL.Model.DTO;

public class MarketplaceUserDTO
{
    public int Id { get; set; }
    public string PictureUrl { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<AddressDTO>? Addresses { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}