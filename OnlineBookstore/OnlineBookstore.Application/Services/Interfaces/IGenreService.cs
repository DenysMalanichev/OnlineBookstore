using OnlineBookstore.Features.GenreFeatures;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IGenreService
{
    Task AddGenreAsync(CreateGenreDto createGenreDto);
    
    Task UpdateGenreAsync(UpdateGenreDto updateGenreDto);

    Task<GetGenreDto> GetGenreByIdAsync(int genreId);

    Task DeleteGenreAsync(int genreId);
}