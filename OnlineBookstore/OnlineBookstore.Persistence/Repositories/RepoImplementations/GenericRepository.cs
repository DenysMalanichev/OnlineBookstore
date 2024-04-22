using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Domain.Common;
using OnlineBookstore.Persistence.Context;
using OnlineBookstore.Persistence.Repositories.Interfaces;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class, IBaseEntity
{
    private readonly DataContext _dbContext;
    private readonly DbSet<T> _dbSet;

    protected GenericRepository(DataContext context)
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

    public async Task AddAsync(T entity)
    {
        _dbSet.Add(entity);
        await _dbContext.SaveChangesAsync();
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

    public async Task AddRangeAsync(IList<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();
    }
}