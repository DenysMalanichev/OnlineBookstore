using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common.Paging;
using OnlineBookstore.Features.AuthorFeatures;

namespace OnlineBookstore.Application.Author.GetAll;

public class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, IEnumerable<GetAuthorDto>>
{
    private readonly IAuthorQueryRepository _authorQueryRepository;
    private readonly IMapper _mapper;

    public GetAllAuthorsQueryHandler(IAuthorQueryRepository authorQueryRepository, IMapper mapper)
    {
        _authorQueryRepository = authorQueryRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<GetAuthorDto>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var authors = await _authorQueryRepository.GetAllAsync();

        var authorDtos = _mapper.Map<IEnumerable<GetAuthorDto>>(authors);

        return authorDtos;
    }
}