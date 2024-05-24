using OnlineBookstore.Application.Common;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Books;

public interface IBookQueryRepository : IGenericQueryRepository<Book>, IQueryingRepository<Book>
{
    (IEnumerable<Book> booksOnPage, int totalItems) GetBooksByAuthor(int authorId, int page, int itemsOnPage);
    
    (IEnumerable<Book> booksOnPage, int totalItems) GetBooksByPublisher(int publisherId, int page, int itemsOnPage);

    double? CountAvgRatingForBook(int bookId);
}