using System.ComponentModel.DataAnnotations.Schema;
using OnlineBookstore.Domain.Common;
using OnlineBookstore.Domain.Constants;

namespace OnlineBookstore.Domain.Entities;

public class Order : IBaseEntity
{
    public string? ShipAddress { get; set; } = null!;

    public string? ShipCity { get; set; } = null!;

    public OrderStatus OrderStatus { get; set; }
    
    public virtual User User { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;

    public virtual IList<OrderDetail> OrderDetails { get; set; } = null!;
    
    public int Id { get; set; }
}