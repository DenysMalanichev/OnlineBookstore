using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;

namespace OnlineBookstore.Application.Books.Delete;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBookCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BookRepository.DeleteAsync(request.BookId);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}