using MediatR;
using OnlineBookstore.Application.Common.Paging;
using OnlineBookstore.Features.AuthorFeatures;

namespace OnlineBookstore.Application.Author.GetAll;

public class GetAllAuthorsQuery : IRequest<IEnumerable<GetAuthorDto>>
{
    
}