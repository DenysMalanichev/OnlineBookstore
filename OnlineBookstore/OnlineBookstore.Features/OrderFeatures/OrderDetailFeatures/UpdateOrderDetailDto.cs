namespace OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;

public class UpdateOrderDetailDto
{
    public int Id { get; set; }
    
    public int BookId { get; set; }

    public int Quantity { get; set; }
}