using MediatR;

namespace OnlineBookstore.Application.Genres.Create;

public class CreateGenreCommand : IRequest
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}