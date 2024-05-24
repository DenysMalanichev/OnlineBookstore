using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.Features.UserFeatures;

public class RegisterUserDto
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string ConfirmPassword { get; set; } = null!;
}