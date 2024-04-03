using Microsoft.AspNetCore.Identity;

namespace OnlineBookstore.Domain.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}