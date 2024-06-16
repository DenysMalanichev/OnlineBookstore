using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Features.BookFeatures;

public class CreateBookDto
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Range(0, int.MaxValue)]
    public decimal Price { get; set; }

    public int AuthorId { get; set; }
    
    public int PublisherId { get; set; }

    public IList<int> GenreIds { get; set; } = null!;
}