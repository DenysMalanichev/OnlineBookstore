using MediatR;

namespace OnlineBookstore.Application.Genres.Update;

public class UpdateGenreCommand : IRequest
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}