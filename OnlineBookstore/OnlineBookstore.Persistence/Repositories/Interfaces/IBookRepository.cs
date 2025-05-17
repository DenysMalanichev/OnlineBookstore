using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Persistence.Repositories.Interfaces;

public interface IBookRepository : IGenericRepository<Book>, IQueryingRepository<Book>
{
    (IEnumerable<Book> booksOnPage, int totalItems) GetBooksByAuthor(int authorId, int page, int itemsOnPage);
    
    (IEnumerable<Book> booksOnPage, int totalItems) GetBooksByPublisher(int publisherId, int page, int itemsOnPage);

    Task<IEnumerable<Book>>? GetByIdAsync(int[] ids, int? page, int itemsOnPage = 10);

    double? CountAvgRatingForBook(int bookId);
}