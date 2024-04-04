using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Features.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
}