using MediatR;

namespace OnlineBookstore.Application.Genres.Delete;

public class DeleteGenreCommand : IRequest
{
    public int GenreId { get; set; }   
}