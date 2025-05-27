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

    public async Task<IEnumerable<Order>> GetOrdersByYearAsync(int[] orderIds, int yearOfInterest)
    {
        return await _dataContext.Orders
            .Where(o => orderIds.Contains(o.Id) &&
                        o.OrderStatus == Domain.Constants.OrderStatus.Closed &&
                        o.OrderClosed.HasValue &&
                        o.OrderClosed!.Value.Year == yearOfInterest)
            .ToListAsync();
    }
}