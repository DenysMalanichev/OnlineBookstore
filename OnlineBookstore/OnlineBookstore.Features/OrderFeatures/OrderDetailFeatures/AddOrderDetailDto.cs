namespace OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;

public class AddOrderDetailDto
{
    public int BookId { get; set; }

    public int Quantity { get; set; }

    public int OrderId { get; set; }
}