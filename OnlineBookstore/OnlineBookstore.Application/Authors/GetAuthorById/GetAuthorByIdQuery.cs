using MediatR;
using OnlineBookstore.Features.AuthorFeatures;

namespace OnlineBookstore.Application.Author.GetAuthorById;

public class GetAuthorByIdQuery : IRequest<GetAuthorDto>
{
    public int AuthorId { get; set; }
}