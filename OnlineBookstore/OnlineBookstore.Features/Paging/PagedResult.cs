namespace OnlineBookstore.Features.Paging;

public class PagedResult<T>
{
    public IQueryable<T> Items { get; set; } = null!;

    public int TotalPages { get; set; }
}