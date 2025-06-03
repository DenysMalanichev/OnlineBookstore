using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Author;
using OnlineBookstore.Application.Author.GetAuthorById;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Features.AuthorFeatures;

namespace OnlineBookstore.Application.Authors.GetAuthorById;

public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, GetAuthorDto>
{
    private readonly IAuthorQueryRepository _authorQueryRepository;
    private readonly IMapper _mapper;

    public GetAuthorByIdQueryHandler(IAuthorQueryRepository authorQueryRepository, IMapper mapper)
    {
        _authorQueryRepository = authorQueryRepository;
        _mapper = mapper;
    }

    public async Task<GetAuthorDto> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = await _authorQueryRepository.GetByIdAsync(request.AuthorId)!
                     ?? throw new EntityNotFoundException($"No Author with Id '{request.AuthorId}'");

        return _mapper.Map<GetAuthorDto>(author);
    }
}