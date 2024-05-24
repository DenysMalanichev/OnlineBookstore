namespace OnlineBookstore.Application.Books.Dtos;

public class GetBookDto
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int AuthorId { get; set; }
    
    public int PublisherId { get; set; }

    public IList<int> GenreIds { get; set; } = null!;
}