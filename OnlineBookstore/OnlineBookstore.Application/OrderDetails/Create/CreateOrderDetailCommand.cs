using MediatR;

namespace OnlineBookstore.Application.OrderDetails.Create;

public class CreateOrderDetailCommand : IRequest
{
    public int BookId { get; set; }

    public int Quantity { get; set; }

    public int OrderId { get; set; }

    public string UserId { get; set; } = null!;
}