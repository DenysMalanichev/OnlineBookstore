using OnlineBookstore.Features.AuthorFeatures;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IAuthorService
{
    Task AddAuthorAsync(CreateAuthorDto createAuthorDto);
    
    Task UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto);

    Task<GetAuthorDto> GetAuthorByIdAsync(int authorDto);

    Task<IEnumerable<GetAuthorDto>> GetAllAuthorsAsync();

    Task DeleteAuthorAsync(int authorDto);
}