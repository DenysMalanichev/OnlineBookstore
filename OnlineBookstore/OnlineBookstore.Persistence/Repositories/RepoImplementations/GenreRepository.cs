using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.Interfaces;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class GenreRepository : GenericRepository<Genre>, IGenreRepository
{
    public GenreRepository(DataContext context)
        : base(context)
    {
    }
}