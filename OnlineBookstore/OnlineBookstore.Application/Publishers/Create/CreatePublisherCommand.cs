using System.ComponentModel.DataAnnotations;
using MediatR;

namespace OnlineBookstore.Application.Publishers.Create;

public class CreatePublisherCommand : IRequest
{
    public string CompanyName { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    [Phone]
    public string? Phone { get; set; }

    public string? Address { get; set; }
}