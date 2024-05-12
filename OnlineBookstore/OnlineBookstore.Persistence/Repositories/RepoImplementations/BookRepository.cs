using System.Data.Entity;
using LinqKit;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;
using OnlineBookstore.Persistence.Repositories.Interfaces;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    private readonly DataContext _dataContext;
    
    public BookRepository(DataContext context)
        : base(context)
    {
        _dataContext = context;
    }

    public IQueryable<Book> GetItemsByPredicate(ExpressionStarter<Book> predicate, bool sortDescending)
    {
        var query = _dataContext.Books
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Where(predicate);
    
        return sortDescending
            ? query.OrderByDescending(b => b.Price) 
            : query;
    }

    public (IEnumerable<Book> booksOnPage, int totalItems) GetBooksByAuthor(int authorId, int page, int itemsOnPage)
    {
        return (_dataContext.Books.Where(b => b.AuthorId == authorId)
            .Skip((page - 1) * itemsOnPage)
            .Take(itemsOnPage)
            .ToList(), _dataContext.Books.Count(b => b.AuthorId == authorId));
    }

    public (IEnumerable<Book> booksOnPage, int totalItems) GetBooksByPublisher(int publisherId, int page, int itemsOnPage)
    {
        return (_dataContext.Books.Where(b => b.PublisherId == publisherId)
            .Skip((page - 1) * itemsOnPage)
            .Take(itemsOnPage)
            .ToList(), _dataContext.Books.Count(b => b.PublisherId == publisherId));
    }

    public double? CountAvgRatingForBook(int bookId)
    {
        try
        {
            return _dataContext.Comments
                .Where(c => c.BookId == bookId)
                .Average(c => c.BookRating);
        }
        catch (InvalidOperationException)
        {
            return null!;
        }
    }
}