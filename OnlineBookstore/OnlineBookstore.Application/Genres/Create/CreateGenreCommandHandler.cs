using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Genres.Create;

public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateGenreCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = _mapper.Map<Genre>(request);

        await _unitOfWork.GenreRepository.AddAsync(genre);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}