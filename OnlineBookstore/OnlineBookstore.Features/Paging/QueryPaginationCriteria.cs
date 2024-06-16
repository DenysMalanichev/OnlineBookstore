using System.Linq.Expressions;

namespace OnlineBookstore.Features.Paging;

public class QueryPaginationCriteria<T> : IPageable
{
    public Expression<Func<T, object>> SortBy { get; set; } = null!;

    public bool IsDescending { get; set; }

    public int ItemsOnPage { get; set; }

    public int CurrentPage { get; set; }
}