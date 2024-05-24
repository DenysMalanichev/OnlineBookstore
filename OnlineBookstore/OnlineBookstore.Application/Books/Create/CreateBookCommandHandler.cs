using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Books.Create;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBookCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = _mapper.Map<Book>(request);

        await _unitOfWork.BookRepository.AddAsync(book);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}