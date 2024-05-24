using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Comments.Dtos;

namespace OnlineBookstore.Application.Comments.GetByBook;

public class GetCommentsByBookQueryHandler : IRequestHandler<GetCommentsByBookQuery, IEnumerable<GetCommentDto>>
{
    private readonly ICommentQueryRepository _commentQueryRepository;
    private readonly IMapper _mapper;

    public GetCommentsByBookQueryHandler(ICommentQueryRepository commentQueryRepository, IMapper mapper)
    {
        _commentQueryRepository = commentQueryRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<GetCommentDto>> Handle(GetCommentsByBookQuery request, CancellationToken cancellationToken)
    {
        var comments = await _commentQueryRepository.GetCommentsByBookIdAsync(request.BookId);
        return _mapper.Map<IEnumerable<GetCommentDto>>(comments);
    }
}