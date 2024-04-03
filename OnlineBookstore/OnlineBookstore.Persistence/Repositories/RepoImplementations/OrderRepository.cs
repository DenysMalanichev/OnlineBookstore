using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(DataContext context)
        : base(context)
    {
    }
}