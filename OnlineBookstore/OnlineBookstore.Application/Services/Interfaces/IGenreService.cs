using OnlineBookstore.Features.GenreFeatures;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IGenreService
{
    Task AddGenreAsync(CreateGenreDto createGenreDto);
    
    Task UpdateGenreAsync(UpdateGenreDto updateGenreDto);

    Task<GetGenreDto> GetGenreByIdAsync(int genreId);

    Task<IEnumerable<GetGenreDto>> GetAllGenresAsync();
    
    Task<IEnumerable<GetBriefGenreDto>> GetGenresByBookAsync(int bookId);

    Task DeleteGenreAsync(int genreId);
}