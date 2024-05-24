using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Orders;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class OrderQueryRepository : GenericQueryRepository<Order>, IOrderQueryRepository
{
    private readonly DataContext _dataContext;
    
    public OrderQueryRepository(DataContext context) : base(context)
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