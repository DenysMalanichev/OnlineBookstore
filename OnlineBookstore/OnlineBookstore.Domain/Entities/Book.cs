using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class Book : IBaseEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [MinLength(2)]
    [MaxLength(2)]
    public string Language { get; set; } = default!;

    public bool IsPaperback { get; set; } = true;

    [Column(TypeName = "decimal(5, 2)")]
    public decimal Rating { get; set; }

    public decimal Price { get; set; }

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    
    public Author Author { get; set; } = null!;

    [ForeignKey(nameof(PublisherId))]
    public int PublisherId { get; set; }
    
    public Publisher Publisher { get; set; } = null!;

    [NotMapped]
    [ForeignKey(nameof(Genres))]
    public int[] GenreIds { get; set; } = null!;
    
    public IList<Genre> Genres { get; set; } = null!;
    
    public int Id { get; set; }

    public byte[]? Image { get; set; } = [];
}