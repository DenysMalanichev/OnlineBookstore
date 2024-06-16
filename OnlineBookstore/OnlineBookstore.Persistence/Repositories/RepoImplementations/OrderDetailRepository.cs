using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;
using OnlineBookstore.Persistence.Repositories.Interfaces;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(DataContext context)
        : base(context)
    {
    }
}