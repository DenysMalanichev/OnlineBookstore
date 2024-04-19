using System.ComponentModel.DataAnnotations.Schema;
using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class OrderDetail : IBaseEntity
{
    public virtual Book Book { get; set; } = null!;

    [ForeignKey(nameof(Book))]
    public int BookId { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;
    
    [ForeignKey(nameof(Order))]
    public virtual int OrderId { get; set; }
    
    public int Id { get; set; }
}