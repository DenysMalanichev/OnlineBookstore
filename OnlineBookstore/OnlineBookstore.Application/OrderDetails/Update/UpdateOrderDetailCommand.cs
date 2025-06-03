using MediatR;

namespace OnlineBookstore.Application.OrderDetails.Update;

public class UpdateOrderDetailCommand : IRequest
{
    public int BookId { get; set; }

    public int Quantity { get; set; }

    public int Id { get; set; }
    
    public int OrderId { get; set; }
}