namespace OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;

public class GetOrderDetailDto
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public int Quantity { get; set; }
}