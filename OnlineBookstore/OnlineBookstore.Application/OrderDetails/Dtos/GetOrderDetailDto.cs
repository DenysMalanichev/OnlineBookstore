namespace OnlineBookstore.Application.OrderDetails.Dtos;

public class GetOrderDetailDto
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public int Quantity { get; set; }
}