using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class OrderDetail : IBaseEntity
{
    public virtual Book Book { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;
    
    public int Id { get; set; }
}