using OnlineBookstore.Features.UserFeatures;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IUserService
{
    Task RegisterUserAsync(RegisterUserDto registerUserDto);
    
    Task RegisterAdminAsync(RegisterUserDto registerUserDto);

    Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginUserDto);
}