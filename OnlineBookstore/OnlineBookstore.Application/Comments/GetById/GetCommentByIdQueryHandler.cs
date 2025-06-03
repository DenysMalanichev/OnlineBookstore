using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Comments.Dtos;
using OnlineBookstore.Application.Exceptions;

namespace OnlineBookstore.Application.Comments.GetById;

public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, GetCommentDto>
{
    private readonly ICommentQueryRepository _commentQueryRepository;
    private readonly IMapper _mapper;

    public GetCommentByIdQueryHandler(ICommentQueryRepository commentQueryRepository, IMapper mapper)
    {
        _commentQueryRepository = commentQueryRepository;
        _mapper = mapper;
    }

    public async Task<GetCommentDto> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var comment = await _commentQueryRepository.GetByIdAsync(request.CommentId)!
                      ?? throw new EntityNotFoundException($"No Comment with Id '{request.CommentId}'");

        var commentDto = _mapper.Map<GetCommentDto>(comment);

        return commentDto;
    }
}