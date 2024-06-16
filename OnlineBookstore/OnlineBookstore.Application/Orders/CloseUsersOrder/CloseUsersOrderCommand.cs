using MediatR;

namespace OnlineBookstore.Application.Orders.CloseUsersOrder;

public class CloseUsersOrderCommand : IRequest
{
    public string ShipAddress { get; set; } = null!;

    public string ShipCity { get; set; } = null!;

    public string UserId { get; set; } = null!;
}