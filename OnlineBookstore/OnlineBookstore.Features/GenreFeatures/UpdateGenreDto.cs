namespace OnlineBookstore.Features.GenreFeatures;

public class UpdateGenreDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}