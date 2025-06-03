using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Domain.Common;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories;

public class GenericQueryRepository<T> : IGenericQueryRepository<T>
    where T : class, IBaseEntity
{
    private readonly DataContext _dbContext;
    private readonly DbSet<T> _dbSet;

    protected GenericQueryRepository(DataContext context)
    {
        _dbContext = context;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id, bool noTracking = false)
    {
        return (noTracking
            ? await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id)
            : (await _dbSet.FindAsync(id))!)!;
    }
}