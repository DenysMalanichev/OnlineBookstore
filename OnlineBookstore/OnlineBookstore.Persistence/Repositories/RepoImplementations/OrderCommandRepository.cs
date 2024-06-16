using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Orders;
using OnlineBookstore.Application.Orders.CloseUsersOrder;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class OrderCommandRepository : GenericRepository<Order>, IOrderCommandRepository
{
    private readonly DataContext _context;
    
    public OrderCommandRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task CloseOrderAsync(CloseOrderData closeOrderData)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == closeOrderData.OrderId)
            ?? throw new EntityNotFoundException($"No order with Id '{closeOrderData.OrderId}'");

        order.ShipCity = closeOrderData.ShipCity;
        order.ShipAddress = closeOrderData.ShipAddress;
        order.OrderStatus = OrderStatus.Closed;

        await _context.SaveChangesAsync();
    }
}