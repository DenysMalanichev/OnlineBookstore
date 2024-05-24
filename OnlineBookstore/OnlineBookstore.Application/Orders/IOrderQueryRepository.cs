using OnlineBookstore.Application.Common;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Orders;

public interface IOrderQueryRepository : IGenericQueryRepository<Order>
{
    Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
}