using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Books.Update;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateBookCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var bookToUpdate = _mapper.Map<Book>(request);

        await _unitOfWork.BookRepository.UpdateAsync(request.AuthorId, bookToUpdate);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}