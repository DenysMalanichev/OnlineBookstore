using System.Linq.Expressions;

namespace OnlineBookstore.Application.Books.GetBooksUsingFilters.BooksSpecifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}