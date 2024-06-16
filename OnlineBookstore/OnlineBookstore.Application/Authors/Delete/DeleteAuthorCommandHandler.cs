using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;

namespace OnlineBookstore.Application.Author.Delete;

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DeleteAuthorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.AuthorRepository.DeleteAsync(request.AuthorId);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}