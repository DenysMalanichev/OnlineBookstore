using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(DataContext context)
        : base(context)
    {
    }
}