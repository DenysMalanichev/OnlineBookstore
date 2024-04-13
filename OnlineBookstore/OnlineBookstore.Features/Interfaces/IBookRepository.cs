using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Features.Interfaces;

public interface IBookRepository : IGenericRepository<Book>, IQueryingRepository<Book>
{
    (IEnumerable<Book> booksOnPage, int totalItems) GetBooksByAuthorAsync(int authorId, int page, int itemsOnPage);
}