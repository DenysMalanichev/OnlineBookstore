using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Books.Dtos;
using OnlineBookstore.Application.Common.Paging;

namespace OnlineBookstore.Application.Books.GetBooksByPublisher;

public class GetBooksByPublisherQueryHandler : IRequestHandler<GetBooksByPublisherQuery, GenericPagingDto<GetBriefBookDto>>
{
    private readonly IBookQueryRepository _bookQueryRepository;
    private readonly IMapper _mapper;

    public GetBooksByPublisherQueryHandler(IBookQueryRepository bookQueryRepository, IMapper mapper)
    {
        _bookQueryRepository = bookQueryRepository;
        _mapper = mapper;
    }
    
    public async Task<GenericPagingDto<GetBriefBookDto>> Handle(GetBooksByPublisherQuery request, CancellationToken cancellationToken)
    {
        var books = _bookQueryRepository.GetBooksByPublisher(request.PublisherId, request.Page ?? 1, request.ItemsOnPage);

        var bookDtos = _mapper.Map<IEnumerable<GetBriefBookDto>>(books.booksOnPage);

        return new GenericPagingDto<GetBriefBookDto>
        {
            CurrentPage = request.Page ?? 1,
            Entities = bookDtos,
            TotalPages = books.totalItems,
        };
    }
}