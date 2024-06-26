namespace OnlineBookstore.Features.PublisherFeatures;

public class GetPublisherDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }
}