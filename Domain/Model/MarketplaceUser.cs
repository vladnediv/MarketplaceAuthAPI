namespace Domain.Model;

public class MarketplaceUser
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public IEnumerable<Address>? Addresses { get; set; }
    //IEnumerable<PaymentMethod> PaymentMethods { get; set; }
}