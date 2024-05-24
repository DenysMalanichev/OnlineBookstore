using OnlineBookstore.Application.Author;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class AuthorQueryRepository : GenericQueryRepository<Author>, IAuthorQueryRepository
{
    public AuthorQueryRepository(DataContext context) : base(context)
    {
    }
}