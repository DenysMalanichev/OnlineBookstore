using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Application.Common;

public interface IGenericQueryRepository<T> where T : class, IBaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    
    Task<T>? GetByIdAsync(int id, bool noTracking = false);
}