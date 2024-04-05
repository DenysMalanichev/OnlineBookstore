using System.ComponentModel.DataAnnotations;
using OnlineBookstore.Domain.Constants;

namespace OnlineBookstore.Features.UserFeatures;

public class LoginResponseDto
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    public string RoleName { get; set; } = null!;
}