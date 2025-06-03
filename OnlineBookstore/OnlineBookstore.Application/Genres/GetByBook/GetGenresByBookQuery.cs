using MediatR;
using OnlineBookstore.Application.Genres.Dtos;

namespace OnlineBookstore.Application.Genres.GetByBook;

public class GetGenresByBookQuery : IRequest<IEnumerable<GetBriefGenreDto>>
{
    public int BookId { get; set; }
}