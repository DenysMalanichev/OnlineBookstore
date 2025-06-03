using MediatR;
using OnlineBookstore.Application.Books.Dtos;
using OnlineBookstore.Application.Common.Paging;

namespace OnlineBookstore.Application.Books.GetBooksByAuthor;

public class GetBooksByAuthorQuery : IRequest<GenericPagingDto<GetBriefBookDto>>
{
    public int AuthorId { get; set; }

    public int? Page { get; set; }

    public int ItemsOnPage { get; set; } = 10;
}