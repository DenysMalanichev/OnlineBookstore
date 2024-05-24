namespace OnlineBookstore.Application.Common.Paging;

public interface IPageable
{
    public int ItemsOnPage { get; set; }

    public int CurrentPage { get; set; }
}