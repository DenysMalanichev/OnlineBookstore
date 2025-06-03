using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.UserFeatures;
using OnlineBookstore.Features.UserFeatures.Options;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace OnlineBookstore.Application.Users;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    private readonly JwtOptions _jwtOptions;

    public UserService(UserManager<User> userManager, IMapper mapper, IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _mapper = mapper;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var user = await CreateUserAsync(registerUserDto);

        await _userManager.AddToRoleAsync(user, RoleName.User.ToString());
    }

    public async Task RegisterAdminAsync(RegisterUserDto registerUserDto)
    {
        var user = await CreateUserAsync(registerUserDto);

        await _userManager.AddToRoleAsync(user, RoleName.Admin.ToString());
    }

    public async Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginUserDto)
    {
        var userExists = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginUserDto.Email)
                         ?? throw new ArgumentException("User doest exists");

        var role = await _userManager.IsInRoleAsync(userExists, RoleName.User.ToString())
            ? RoleName.User
            : RoleName.Admin;

        return await _userManager.CheckPasswordAsync(userExists, loginUserDto.Password)
            ? new LoginResponseDto
            {
                Token = GenerateJwtAsync(userExists, role.ToString())
            }
            : throw new AuthenticationException("Wrong password");
    }

    public async Task<GetUserDto> GetUserDataAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
                   ?? throw new ArgumentException("User doest exists");

        var userDto = _mapper.Map<GetUserDto>(user);

        return userDto;
    }

    private string GenerateJwtAsync(User user, string roleName)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, roleName),
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret));

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            expires: DateTime.UtcNow.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return $"Bearer {tokenValue}";
    }

    private async Task<User> CreateUserAsync(RegisterUserDto registerUserDto)
    {
        if (registerUserDto.Password != registerUserDto.ConfirmPassword)
        {
            throw new ArgumentException("Password is not equal to Confirm Password");
        }

        var userExists = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == registerUserDto.Email);

        if (userExists is not null)
        {
            throw new ArgumentException("User with this Name already exists.");
        }

        var newUser = _mapper.Map<User>(registerUserDto)
                      ?? throw new ArgumentException("Invalid data.");

        var result = await _userManager.CreateAsync(newUser, registerUserDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Aggregate(string.Empty, (current, err) => current + err.Description);

            throw new ArgumentException($"User cannot be created: {errors}");
        }

        return newUser;
    }
}