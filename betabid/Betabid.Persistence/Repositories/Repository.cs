using System.Linq.Expressions;
using Betabid.Application.Interfaces.Repositories;
using Betabid.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DataContext _dbContext;
    private readonly DbSet<T> _dbSet;

    protected Repository(DataContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        _dbSet.Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return Task.FromResult(_dbContext.Set<T>().Any(predicate));
    }
}