using MediatR;

namespace OnlineBookstore.Application.Publishers.Update;

public class UpdatePublisherCommand : IRequest
{
    public string CompanyName { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }
    
    public int Id { get; set; }
}