using System.ComponentModel.DataAnnotations.Schema;
using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class Book : IBaseEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal Price { get; set; }

    public int AuthorId { get; set; }

    public Author Author { get; set; } = null!;

    public int PublisherId { get; set; }
    
    public Publisher Publisher { get; set; } = null!;

    public IList<Genre> Genres { get; set; } = null!;
    
    public int Id { get; set; }
}