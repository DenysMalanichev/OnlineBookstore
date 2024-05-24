using MediatR;
using OnlineBookstore.Application.Books.Dtos;

namespace OnlineBookstore.Application.Books.GetById;

public class GetBookByIdQuery : IRequest<GetBookDto>
{
    public int BookId { get; set; }
}