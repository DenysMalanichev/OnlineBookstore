using OnlineBookstore.Application.OrderDetails;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class OrderDetailQueryRepository : GenericQueryRepository<OrderDetail>, IOrderDetailQueryRepository
{
    public OrderDetailQueryRepository(DataContext context)
        : base(context)
    {
    }
}