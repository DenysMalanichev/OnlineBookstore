using OnlineBookstore.Features.BookFeatures;
using OnlineBookstore.Features.Paging;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IBookService
{
    Task AddBookAsync(CreateBookDto createBookDto);
    
    Task UpdateBookAsync(UpdateBookDto updateBookDto);

    Task<GetBookDto> GetBookByIdAsync(int bookId);

    Task<GenericPagingDto<GetBookDto>> GetBooksUsingFiltersAsync(GetFilteredBooksDto filteredBooksDto);

    Task DeleteBookAsync(int bookId);
}