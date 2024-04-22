using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;
using OnlineBookstore.Persistence.Repositories.Interfaces;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(DataContext context)
        : base(context)
    {
    }
}