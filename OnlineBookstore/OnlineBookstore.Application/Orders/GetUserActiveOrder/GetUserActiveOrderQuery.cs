using MediatR;
using OnlineBookstore.Application.Orders.Dtos;

namespace OnlineBookstore.Application.Orders.GetUserActiveOrder;

public class GetUserActiveOrderQuery : IRequest<GetOrderDto>
{
    public string UserId { get; set; }
}