using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Domain.Common;
using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class GenericRepository<T> : IGenericRepository<T>
    where T : BaseEntity
{
    private readonly DataContext _context;

    private readonly DbSet<T> _table;

    protected GenericRepository(DataContext context)
    {
        _context = context;
        _table = _context.Set<T>();
    }

    public async Task<T>? GetByIdAsync(int id)
    {
        return (await _table.FirstOrDefaultAsync(g => g.Id == id))!;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _table.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        var added = await _table.AddAsync(entity);

        return added.Entity;
    }

    public async Task<T> UpdateAsync(int id, T entity)
    {
        var dbEntity = await GetByIdAsync(id)!;

        if (dbEntity is not null)
        {
            entity.Id = dbEntity.Id;
            _context.Entry(dbEntity).CurrentValues.SetValues(entity);
        }

        return dbEntity;
    }

    public async Task<T>? DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id)!;

        if (entity is null)
        {
            return null!;
        }

        _table.Remove(entity);

        return entity;
    }
}