using LinqKit;

namespace OnlineBookstore.Persistence.Repositories.Interfaces;

public interface IQueryingRepository<T>
{
    IQueryable<T> GetItemsByPredicate(ExpressionStarter<T> predicate, bool sortDescending);
}