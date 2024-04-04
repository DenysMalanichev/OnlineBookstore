using OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;

namespace OnlineBookstore.Features.OrderFeatures;

public class GetOrderDto
{
    public string ShipAddress { get; set; } = null!;

    public string ShipCity { get; set; } = null!;

    public IEnumerable<GetOrderDetailDto> OrderDetails { get; set; }

    public int Id { get; set; }
}