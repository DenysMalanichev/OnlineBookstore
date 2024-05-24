namespace OnlineBookstore.Application.Genres.Dtos;

public class GetGenreDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}