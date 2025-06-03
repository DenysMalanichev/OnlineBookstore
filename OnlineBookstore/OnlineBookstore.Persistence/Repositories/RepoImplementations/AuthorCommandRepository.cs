using OnlineBookstore.Application.Author;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class AuthorCommandRepository : GenericRepository<Author>, IAuthorCommandRepository
{
    public AuthorCommandRepository(DataContext context)
        : base(context)
    {
    }
}