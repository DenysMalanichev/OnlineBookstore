using OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;

namespace OnlineBookstore.Features.OrderFeatures;

public class GetOrderDto
{
    public string ShipAddress { get; set; } = null!;

    public string ShipCity { get; set; } = null!;

    public IEnumerable<GetOrderDetailDto> OrderDetails { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? OrderClosed { get; set; } = null!;

    public int Id { get; set; }
}