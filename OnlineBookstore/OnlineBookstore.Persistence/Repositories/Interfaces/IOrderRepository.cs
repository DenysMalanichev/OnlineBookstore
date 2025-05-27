using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Persistence.Repositories.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);

    Task<IEnumerable<Order>> GetOrdersByYearAsync(int[] bookIds, int yearOfInterest);
}