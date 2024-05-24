namespace OnlineBookstore.Application.Books.Dtos;

public class GetBriefBookDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string AuthorName { get; set; } = null!;
}