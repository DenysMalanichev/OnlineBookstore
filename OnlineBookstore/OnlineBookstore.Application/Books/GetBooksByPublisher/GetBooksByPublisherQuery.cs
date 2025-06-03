using MediatR;
using OnlineBookstore.Application.Books.Dtos;
using OnlineBookstore.Application.Common.Paging;

namespace OnlineBookstore.Application.Books.GetBooksByPublisher;

public class GetBooksByPublisherQuery : IRequest<GenericPagingDto<GetBriefBookDto>>
{
    public int PublisherId { get; set; }

    public int? Page { get; set; }

    public int ItemsOnPage { get; set; } = 10;
}