using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(DataContext context)
        : base(context)
    {
    }
}