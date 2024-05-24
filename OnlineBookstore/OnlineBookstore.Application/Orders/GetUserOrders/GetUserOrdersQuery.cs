using MediatR;
using OnlineBookstore.Application.Orders.Dtos;

namespace OnlineBookstore.Application.Orders.GetUserOrders;

public class GetUserOrdersQuery : IRequest<IEnumerable<GetOrderDto>>
{
    public string UserId { get; set; } = null!;
}