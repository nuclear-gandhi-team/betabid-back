using System.Linq.Expressions;

namespace Betabid.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    
    Task<IEnumerable<T>> GetAllAsync();
    
    Task<T> AddAsync(T entity);
    
    Task UpdateAsync(T entity);
    
    Task DeleteAsync(T entity);
    
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}