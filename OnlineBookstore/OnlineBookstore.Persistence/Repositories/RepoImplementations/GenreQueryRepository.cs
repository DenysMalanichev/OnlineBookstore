using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Application.Exceptions;
using OnlineBookstore.Application.Genres;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Persistence.Repositories.RepoImplementations;

public class GenreQueryRepository : GenericQueryRepository<Genre>, IGenreQueryRepository
{
    private readonly DataContext _dataContext;

    public GenreQueryRepository(DataContext context, DataContext dataContext) : base(context)
    {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<Genre>> GetGenresByBookAsync(int bookId)
    {
        var book = (await _dataContext.Books
                       .Include(b => b.Genres)
                       .FirstOrDefaultAsync(b => b.Id == bookId))
                   ?? throw new EntityNotFoundException($"No Book found with Id '{bookId}'");

        return book.Genres;
    }
}