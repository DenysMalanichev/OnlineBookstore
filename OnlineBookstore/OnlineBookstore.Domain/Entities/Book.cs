using System.ComponentModel.DataAnnotations.Schema;
using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class Book : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal Price { get; set; }

    public virtual Author Author { get; set; } = null!;
    
    public virtual Publisher Publisher { get; set; } = null!;

    public virtual IList<Genre> Genres { get; set; } = null!;
}