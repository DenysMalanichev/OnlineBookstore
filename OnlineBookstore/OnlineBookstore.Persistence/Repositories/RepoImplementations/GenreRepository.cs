using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;
using OnlineBookstore.Persistence.Repositories.Interfaces;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class GenreRepository : GenericRepository<Genre>, IGenreRepository
{
    public GenreRepository(DataContext context)
        : base(context)
    {
    }
}