using System.ComponentModel.DataAnnotations;
using MediatR;

namespace OnlineBookstore.Application.Author.Create;

public class CreateAuthorCommand : IRequest
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    [EmailAddress]
    public string? Email { get; set; } = null!;
}