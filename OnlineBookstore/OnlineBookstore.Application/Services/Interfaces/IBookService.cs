using OnlineBookstore.Features.BookFeatures;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IBookService
{
    Task AddBookAsync(CreateBookDto createBookDto);
    
    Task UpdateBookAsync(UpdateBookDto updateBookDto);

    Task<GetBookDto> GetBookByIdAsync(int bookId);

    Task DeleteBookAsync(int bookId);
}