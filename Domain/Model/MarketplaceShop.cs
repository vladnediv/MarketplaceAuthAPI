namespace Domain.Model;

//TODO Look at epicentr and analyze shop model
public class MarketplaceShop
{
    public int Id { get; set; }
    
    public int ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    
    public string LogoUrl { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    IEnumerable<int> ProductIds { get; set; }
}