using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Features.Interfaces;

public interface IBookRepository : IGenericRepository<Book>, IQueryingRepository<Book>
{
}