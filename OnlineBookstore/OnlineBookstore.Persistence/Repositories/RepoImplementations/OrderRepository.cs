using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;
using OnlineBookstore.Persistence.Repositories.Interfaces;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly DataContext _dataContext;
    
    public OrderRepository(DataContext context)
        : base(context)
    {
        _dataContext = context;
    }

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
    {
        return await _dataContext.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .Where(o => o.User.Id == userId)
            .ToListAsync();
    }
}