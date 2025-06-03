using MediatR;

namespace OnlineBookstore.Application.OrderDetails.Delete;

public class DeleteOrderDetailCommand : IRequest
{
    public int OrderDetailId { get; set; }
}