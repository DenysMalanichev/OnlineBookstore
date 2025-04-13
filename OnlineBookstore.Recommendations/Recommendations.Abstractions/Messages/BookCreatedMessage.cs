namespace Recommendations.Abstractions.Messages;
public record BookCreatedMessage
{
    public int BookId { get; set; }
    public string Title { get; set; } = default!;
    public string Language { get; set; } = default!;
    public List<int> GenreIds { get; set; } = default!;
    public int AuthorId { get; set; }
    public double Rating { get; set; }
    public int PurchaseNumber { get; set; }
    public bool IsPaperback { get; set; }
};
