using OnlineBookstore.Application.Common;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Genres;

public interface IGenreQueryRepository : IGenericQueryRepository<Genre>
{
    Task<IEnumerable<Genre>> GetGenresByBookAsync(int bookId);
}