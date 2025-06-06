namespace OnlineBookstore.Features.BookFeatures;

public class GetBookDto
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int AuthorId { get; set; }
    
    public int PublisherId { get; set; }

    public IList<int> GenreIds { get; set; } = null!;

    public string Language { get; set; } = default!;

    public bool IsPaperback { get; set; }
}