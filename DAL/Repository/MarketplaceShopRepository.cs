using DAL.Context;
using DAL.Repository.Interface;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class MarketplaceShopRepository : IGenericRepository<MarketplaceShop>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<MarketplaceShop> _dbSet;

    public MarketplaceShopRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<MarketplaceShop>();
    }

    public async Task<MarketplaceShop> Create(MarketplaceShop entity)
    {
        var res = await _dbSet.AddAsync(entity);
        return res.Entity;
    }

    public async Task<MarketplaceShop?> GetById(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(shop => shop.Id == id);
    }

    public async Task Update(MarketplaceShop entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task Delete(MarketplaceShop entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}