using AutoMapper;
using MediatR;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Genres.Dtos;

namespace OnlineBookstore.Application.Genres.GetByBook;

public class GetGenresByBookQueryHandler : IRequestHandler<GetGenresByBookQuery, IEnumerable<GetBriefGenreDto>>
{
    private readonly IGenreQueryRepository _genreQueryRepository;
    private readonly IMapper _mapper;

    public GetGenresByBookQueryHandler(IGenreQueryRepository genreQueryRepository, IMapper mapper)
    {
        _mapper = mapper;
        _genreQueryRepository = genreQueryRepository;
    }

    
    public async Task<IEnumerable<GetBriefGenreDto>> Handle(GetGenresByBookQuery request, CancellationToken cancellationToken)
    {
        var genres = await _genreQueryRepository.GetGenresByBookAsync(request.BookId);

        var genresDtos = _mapper.Map<IEnumerable<GetBriefGenreDto>>(genres);

        return genresDtos;
    }
}