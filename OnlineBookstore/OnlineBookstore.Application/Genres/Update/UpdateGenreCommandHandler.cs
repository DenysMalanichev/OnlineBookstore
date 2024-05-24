using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Genres.Update;

public class UpdateGenreCommandHandler : IRequestHandler<UpdateGenreCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateGenreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
    {
        var genreToUpdate = _mapper.Map<Genre>(request);

        await _unitOfWork.GenreRepository.UpdateAsync(request.Id, genreToUpdate);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}