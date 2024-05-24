using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;

namespace OnlineBookstore.Application.Genres.Delete;

public class DeleteGenreCommandHandler : IRequestHandler<DeleteGenreCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DeleteGenreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.GenreRepository.DeleteAsync(request.GenreId);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}