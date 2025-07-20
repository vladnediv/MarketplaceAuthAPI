using Domain.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<MarketplaceUser> MarketplaceUsers { get; set; }
    public DbSet<MarketplaceShop> MarketplaceShops { get; set; }
    public DbSet<MarketplaceAdmin> MarketplaceAdmins { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        //ApplicationUser - MarketplaceUser : One-To-One relation
        builder.Entity<ApplicationUser>()
            .HasOne(x => x.MarketplaceUser)
            .WithOne(x => x.ApplicationUser)
            .HasForeignKey<MarketplaceUser>(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        //ApplicationUser - MarketplaceShop : One-To-One relation
        builder.Entity<ApplicationUser>()
            .HasOne(x => x.MarketplaceShop)
            .WithOne(x => x.ApplicationUser)
            .HasForeignKey<MarketplaceShop>(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        //ApplicationUser - MarketplaceAdmin : One-To-One relation
        builder.Entity<ApplicationUser>()
            .HasOne(x => x.MarketplaceAdmin)
            .WithOne(x => x.ApplicationUser)
            .HasForeignKey<MarketplaceAdmin>(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}