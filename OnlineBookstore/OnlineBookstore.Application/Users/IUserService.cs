using OnlineBookstore.Features.UserFeatures;

namespace OnlineBookstore.Application.Users;

public interface IUserService
{
    Task RegisterUserAsync(RegisterUserDto registerUserDto);
    
    Task RegisterAdminAsync(RegisterUserDto registerUserDto);

    Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginUserDto);

    Task<GetUserDto> GetUserDataAsync(string userId);
}