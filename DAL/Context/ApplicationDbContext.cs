using Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser,  IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<MarketplaceUser> MarketplaceUsers { get; set; }
    public DbSet<MarketplaceShop> MarketplaceShops { get; set; }
    public DbSet<MarketplaceAdmin> MarketplaceAdmins { get; set; }
    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        //MarketplaceUser - ApplicationUser : One-To-One relation
        builder.Entity<MarketplaceUser>()
            .HasOne(mu => mu.ApplicationUser)
            .WithOne(x => x.MarketplaceUser)
            .HasForeignKey<MarketplaceUser>(mu => mu.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        //MarketplaceShop - ApplicationUser : One-To-One relation
        builder.Entity<MarketplaceShop>()
            .HasOne(ms => ms.ApplicationUser)
            .WithOne(x => x.MarketplaceShop)
            .HasForeignKey<MarketplaceShop>(ms => ms.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        //MarketplaceAdmin - ApplicationUser : One-To-One relation
        builder.Entity<MarketplaceAdmin>()
            .HasOne(ma => ma.ApplicationUser)
            .WithOne(x => x.MarketplaceAdmin)
            .HasForeignKey<MarketplaceAdmin>(ma => ma.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        //Address -> MarketplaceUser : One-To-Many relation
        builder.Entity<MarketplaceUser>()
            .HasMany(x => x.Addresses)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //Address -> MarketplaceShop : One-To-One relation
        builder.Entity<MarketplaceShop>()
            .HasOne(x => x.Address)
            .WithOne(x => x.Shop)
            .HasForeignKey<Address>(x => x.ShopId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}