using MediatR;
using OnlineBookstore.Application.Genres.Dtos;

namespace OnlineBookstore.Application.Genres.GetAll;

public class GetAllGenresQuery : IRequest<IEnumerable<GetGenreDto>>
{
}