using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Persistence.Repositories.Interfaces;

public interface IGenericRepository<T>
    where T : class, IBaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    
    Task<T>? GetByIdAsync(int id, bool noTracking = false);

    Task AddAsync(T entity);

    Task AddRangeAsync(IList<T> entities);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}