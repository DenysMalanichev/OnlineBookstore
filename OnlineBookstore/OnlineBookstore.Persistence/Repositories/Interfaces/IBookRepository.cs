using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Persistence.Repositories.Interfaces;

public interface IBookRepository : IGenericRepository<Book>, IQueryingRepository<Book>
{
    (IEnumerable<Book> booksOnPage, int totalItems) GetBooksByAuthorAsync(int authorId, int page, int itemsOnPage);
    
    (IEnumerable<Book> booksOnPage, int totalItems) GetBooksByPublisher(int publisherId, int page, int itemsOnPage);

    double? CountAvgRatingForBook(int bookId);
}