using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Features.AuthorFeatures;

public class UpdateAuthorDto
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;
    
    public int Id { get; set; }
}