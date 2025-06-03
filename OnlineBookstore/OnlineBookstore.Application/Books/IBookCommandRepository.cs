using OnlineBookstore.Application.Common;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Application.Books;

public interface IBookCommandRepository : IGenericRepository<Book>
{
    
}