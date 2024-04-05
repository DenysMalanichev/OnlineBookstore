using LinqKit;

namespace OnlineBookstore.Features.Interfaces;

public interface IQueryingRepository<T>
{
    IQueryable<T> GetItemsByPredicate(ExpressionStarter<T> predicate, bool sortDescending);
}