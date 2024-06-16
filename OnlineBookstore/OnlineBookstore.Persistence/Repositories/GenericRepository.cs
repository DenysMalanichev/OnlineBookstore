using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Domain.Common;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories;

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

    public async Task AddAsync(T entity)
    {
        _dbSet.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        if (await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == entity.Id) is null)
        {
            throw new EntityNotFoundException($"No {typeof(T).Name} found with Id '{entity.Id}'");
        }
        
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int entityId)
    {
        var entity = await _dbSet.FindAsync(entityId)
            ?? throw new EntityNotFoundException($"No {typeof(T).Name} found with Id '{entityId}'");
        
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IList<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();
    }
}