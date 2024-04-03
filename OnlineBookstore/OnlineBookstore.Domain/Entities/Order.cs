using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class Order : BaseEntity
{
    public string ShipAddress { get; set; } = null!;

    public string ShipCity { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}