using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Books.Dtos;
using OnlineBookstore.Application.Exceptions;

namespace OnlineBookstore.Application.Books.GetById;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, GetBookDto>
{
    private readonly IBookQueryRepository _bookQueryRepository;
    private readonly IMapper _mapper;

    public GetBookByIdQueryHandler(IBookQueryRepository bookQueryRepository, IMapper mapper)
    {
        _bookQueryRepository = bookQueryRepository;
        _mapper = mapper;
    }

    public async Task<GetBookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookQueryRepository.GetByIdAsync(request.BookId)!
                   ?? throw new EntityNotFoundException($"No Book with Id '{request.BookId}'");

        return _mapper.Map<GetBookDto>(book);
    }
}