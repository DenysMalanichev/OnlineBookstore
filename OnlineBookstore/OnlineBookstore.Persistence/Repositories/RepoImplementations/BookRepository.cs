using System.Data.Entity;
using LinqKit;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Persistence.Context;

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
            ? query.OrderDescending() 
            : query;
    }
}