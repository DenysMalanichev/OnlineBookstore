using LinqKit;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Books;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class BookCommandRepository : GenericRepository<Book>, IBookCommandRepository
{
    public BookCommandRepository(DataContext context) : base(context)
    {
    }
}