using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Users;
using OnlineBookstore.Extentions;
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

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetUserDataAsync()
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        var userDto = await _userService.GetUserDataAsync(userId);

        return Ok(userDto);
    }
    
    [HttpGet("is-admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult IsAdmin()
    {
        var isAdmin = User.IsInRole("Admin");
        return Ok(isAdmin);
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