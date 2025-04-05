namespace Recommendations.Abstractions.Entities;
public record BookPortrait
{
    public int BookId { get; set; }
    public string Language { get; set; } = default!;
    public IList<int> GenreIds { get; set; } = [];
    public int AuthorId { get; set; }
    public double Rating { get; set; }
    public int PurchaseNumber { get; set; }
    public bool IsPaperback { get; set; } = false;
}
