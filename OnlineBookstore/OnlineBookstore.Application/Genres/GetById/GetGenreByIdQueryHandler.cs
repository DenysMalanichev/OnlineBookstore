using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Genres.Dtos;

namespace OnlineBookstore.Application.Genres.GetById;

public class GetGenreByIdQueryHandler : IRequestHandler<GetGenreByIdQuery, GetGenreDto>
{
    private readonly IGenreQueryRepository _genreQueryRepository;
    private readonly IMapper _mapper;

    public GetGenreByIdQueryHandler(IGenreQueryRepository genreQueryRepository, IMapper mapper)
    {
        _mapper = mapper;
        _genreQueryRepository = genreQueryRepository;
    }
    
    public async Task<GetGenreDto> Handle(GetGenreByIdQuery request, CancellationToken cancellationToken)
    {
        var genre = await _genreQueryRepository.GetByIdAsync(request.GenreId)!
                    ?? throw new EntityNotFoundException($"No Genre with Id '{request.GenreId}'");

        return _mapper.Map<GetGenreDto>(genre);
    }
}