using System.Data.Entity;
using System.Linq;
using Azure;
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

    public async Task<IEnumerable<Book>>? GetByIdAsync(int[] ids, int? page, int itemsOnPage = 10)
    {
        return await Task.FromResult(_dataContext.Books
            .Where(b => ids.Contains(b.Id))
            .Skip(((page ?? 1) - 1) * itemsOnPage)
            .Take(itemsOnPage)
            .AsNoTracking()
            .ToList());            
    }

    public async Task SetBookImageAsync(byte[] bytes, int bookId)
    {
        var book = await Task.FromResult(_dataContext.Books.FirstOrDefault(b => b.Id == bookId));

        if (book is null)
        {
            throw new ArgumentException("No book with Id bookId found");
        }

        book.Image = bytes;
    }

    public async Task<byte[]> GetBookImageAsync(int bookId)
    {
        var book = await Task.FromResult(_dataContext.Books.FirstOrDefault(b => b.Id == bookId));

        if (book is null)
        {
            throw new ArgumentException("No book with Id bookId found");
        }

        return book.Image!;
    }

    public async Task<IEnumerable<Book>>? GetByIdsAsync(int[] ids)
    {
        return await Task.FromResult(_dataContext.Books
            .Where(b => ids.Contains(b.Id))
            .AsNoTracking()
            .ToList());
    }
}