using Microsoft.AspNetCore.Http;
using OnlineBookstore.Features.BookFeatures;
using OnlineBookstore.Features.Paging;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IBookService
{
    Task AddBookAsync(CreateBookDto createBookDto);
    
    Task UpdateBookAsync(UpdateBookDto updateBookDto);

    Task<GenericPagingDto<GetBriefBookDto>> GetRecommendationsAsync(Guid userId, int? page, int itemsOnPage = 10);

    Task<GetBookDto> GetBookByIdAsync(int bookId);

    Task<GenericPagingDto<GetBriefBookDto>> GetBooksUsingFiltersAsync(GetFilteredBooksDto filteredBooksDto);

    GenericPagingDto<GetBriefBookDto> GetBooksByAuthor(int authorId, int? page, int itemsOnPage = 10);
    
    GenericPagingDto<GetBriefBookDto> GetBooksByPublisher(int publisherId, int? page, int itemsOnPage = 10);

    Task DeleteBookAsync(int bookId);

    double? CountAvgRatingOfBook(int bookId);

    Task SetBookImageAsync(IFormFile image, int bookId);

    Task<byte[]?> GetBookImageAsync(int bookId);
}