using DAL.Context;
using DAL.Repository.Interface;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class MarketplaceAdminRepository : IGenericRepository<MarketplaceAdmin>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<MarketplaceAdmin> _dbSet;

    public MarketplaceAdminRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<MarketplaceAdmin>();
    }

    public async Task<MarketplaceAdmin> Create(MarketplaceAdmin entity)
    {
        var res = await _dbSet.AddAsync(entity);
        return res.Entity;
    }

    public async Task<MarketplaceAdmin?> GetById(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(admin => admin.Id == id);
    }

    public async Task Update(MarketplaceAdmin entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task Delete(MarketplaceAdmin entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}