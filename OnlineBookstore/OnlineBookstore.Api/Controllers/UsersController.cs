using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Features.UserFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        await _userService.RegisterUserAsync(registerUserDto);

        return Ok();
    }

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdminAsync(RegisterUserDto registerUserDto)
    {
        await _userService.RegisterAdminAsync(registerUserDto);

        return Ok();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginUserDto loginUserDto)
    {
        var loginResponse = await _userService.LoginUserAsync(loginUserDto);

        return Ok(loginResponse);
    }
}