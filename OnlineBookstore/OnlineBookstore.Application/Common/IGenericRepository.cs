using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Application.Common;

public interface IGenericRepository<T>
    where T : class, IBaseEntity
{
    Task AddAsync(T entity);

    Task AddRangeAsync(IList<T> entities);

    Task UpdateAsync(int entityId, T entity);

    Task DeleteAsync(int entityId);
}