using DAL.Context;
using DAL.Repository.Interface;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class MarketplaceUserRepository : IGenericRepository<MarketplaceUser>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<MarketplaceUser> _dbSet;

    public MarketplaceUserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<MarketplaceUser>();
    }
    
    public async Task Create(MarketplaceUser entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task<MarketplaceUser> GetById(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Update(MarketplaceUser entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task Delete(MarketplaceUser entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task Delete(int id)
    {
        MarketplaceUser? entity = await GetById(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}