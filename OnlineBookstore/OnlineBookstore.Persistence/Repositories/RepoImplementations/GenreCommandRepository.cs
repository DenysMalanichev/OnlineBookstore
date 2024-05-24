using OnlineBookstore.Application.Genres;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class GenreCommandRepository : GenericRepository<Genre>, IGenreCommandRepository
{
    public GenreCommandRepository(DataContext context) : base(context)
    {
    }
}