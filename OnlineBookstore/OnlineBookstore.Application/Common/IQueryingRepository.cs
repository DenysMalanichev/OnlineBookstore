using LinqKit;

namespace OnlineBookstore.Application.Common;

public interface IQueryingRepository<T>
{
    IQueryable<T> GetItemsByPredicate(ExpressionStarter<T> predicate, bool sortDescending);
}