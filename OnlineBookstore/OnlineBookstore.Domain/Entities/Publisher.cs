using OnlineBookstore.Domain.Common;

namespace OnlineBookstore.Domain.Entities;

public class Publisher : BaseEntity
{
    public string CompanyName { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }
}