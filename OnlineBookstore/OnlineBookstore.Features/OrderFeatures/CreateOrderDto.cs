namespace OnlineBookstore.Features.OrderFeatures;

public class CreateOrderDto
{
    public string ShipAddress { get; set; } = null!;

    public string ShipCity { get; set; } = null!;
}