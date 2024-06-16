using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Features.BookFeatures;

public class GetFilteredBooksDto
{
    public string? Name { get; set; } = null!;

    public string? AuthorName { get; set; } = null!;

    public int? PublisherId { get; set; }

    public bool? IsDescending { get; set; }
    
    [Range(0, int.MaxValue)]
    public decimal? MinPrice { get; set; }

    [Range(0, int.MaxValue)]
    public decimal? MaxPrice { get; set; }
    
    public int? Page { get; set; }

    public int? ItemsOnPage { get; set; }
    
    public IList<int>? Genres { get; set; } = null!;
}