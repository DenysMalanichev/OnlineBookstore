using System.Linq.Expressions;

namespace OnlineBookstore.Features.BookFeatures.BooksSpecifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}