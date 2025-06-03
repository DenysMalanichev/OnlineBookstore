using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Comments.Create;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = _mapper.Map<Comment>(request);
        comment.UserId = request.UserId;

        if (_unitOfWork.CommentRepository.IsUserWroteCommentForThisBook(request.UserId, request.BookId))
        {
            throw new UserAlreadyWroteCommentForBookException();
        }
        
        await _unitOfWork.CommentRepository.AddAsync(comment);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}