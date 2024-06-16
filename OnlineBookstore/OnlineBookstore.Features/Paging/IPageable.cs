namespace OnlineBookstore.Features.Paging;

public interface IPageable
{
    public int ItemsOnPage { get; set; }

    public int CurrentPage { get; set; }
}