namespace OnlineBookstore.Application.Orders.CloseUsersOrder;

public record CloseOrderData
{
    public string ShipAddress { get; set; } = null!;

    public string ShipCity { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int OrderId { get; set; }
}