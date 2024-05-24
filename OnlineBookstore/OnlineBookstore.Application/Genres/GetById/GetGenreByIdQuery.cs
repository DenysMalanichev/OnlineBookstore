using MediatR;
using OnlineBookstore.Application.Genres.Dtos;

namespace OnlineBookstore.Application.Genres.GetById;

public class GetGenreByIdQuery : IRequest<GetGenreDto>
{
    public int GenreId { get; set; }    
}