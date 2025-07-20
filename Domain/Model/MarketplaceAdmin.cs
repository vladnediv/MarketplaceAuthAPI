namespace Domain.Model;

public class MarketplaceAdmin
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public int ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}