using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Features.PublisherFeatures;

public class CreatePublisherDto
{
    public string CompanyName { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    [Phone]
    public string? Phone { get; set; }

    public string? Address { get; set; }
}