using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Features.BookFeatures;

public class UpdateBookDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Range(0, int.MaxValue)]
    public decimal Price { get; set; }

    public int AuthorId { get; set; }
    
    public int PublisherId { get; set; }

    public IList<int> GenreIds { get; set; } = null!;

    public string Language { get; set; } = default!;

    public bool IsPaperback { get; set; }
}