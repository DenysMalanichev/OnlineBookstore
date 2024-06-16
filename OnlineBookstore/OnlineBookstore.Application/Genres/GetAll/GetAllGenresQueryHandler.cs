using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Genres.Dtos;

namespace OnlineBookstore.Application.Genres.GetAll;

public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, IEnumerable<GetGenreDto>>
{
    private readonly IGenreQueryRepository _genreQueryRepository;
    private readonly IMapper _mapper;

    public GetAllGenresQueryHandler(IGenreQueryRepository genreQueryRepository, IMapper mapper)
    {
        _mapper = mapper;
        _genreQueryRepository = genreQueryRepository;
    }
    
    public async Task<IEnumerable<GetGenreDto>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
    {
        var genres = await _genreQueryRepository.GetAllAsync();

        var genresDtos = _mapper.Map<IEnumerable<GetGenreDto>>(genres);

        return genresDtos;
    }
}