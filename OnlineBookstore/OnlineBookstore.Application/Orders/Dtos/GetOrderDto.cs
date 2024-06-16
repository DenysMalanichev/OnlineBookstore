using OnlineBookstore.Application.OrderDetails.Dtos;

namespace OnlineBookstore.Application.Orders.Dtos;

public class GetOrderDto
{
    public string ShipAddress { get; set; } = null!;

    public string ShipCity { get; set; } = null!;

    public IEnumerable<GetOrderDetailDto> OrderDetails { get; set; } = null!;

    public string Status { get; set; } = null!;
    
    public int Id { get; set; }
}