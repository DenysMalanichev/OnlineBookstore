namespace OnlineBookstore.Application.Common.Paging;

public class GenericPagingDto<T>
{
    public IEnumerable<T> Entities { get; set; } = null!;

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }
}