using OnlineBookstore.Application.OrderDetails;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class OrderDetailCommandRepository : GenericRepository<OrderDetail>, IOrderDetailCommandRepository
{
    public OrderDetailCommandRepository(DataContext context) : base(context)
    {
    }
}