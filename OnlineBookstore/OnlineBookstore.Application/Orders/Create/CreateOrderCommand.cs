using MediatR;

namespace OnlineBookstore.Application.Orders.Create;

public class CreateOrderCommand : IRequest
{
    public string UserId { get; set; } = null!;
}