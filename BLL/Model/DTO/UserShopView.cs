using Domain.Model;

namespace BLL.Model.DTO;

public class UserShopView
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<AddressDTO>? Addresses { get; set; }
}