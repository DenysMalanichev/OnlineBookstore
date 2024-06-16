using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Orders.CloseUsersOrder;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Orders;

public interface IOrderCommandRepository : IGenericRepository<Order>
{
    Task CloseOrderAsync(CloseOrderData closeOrderData);
}