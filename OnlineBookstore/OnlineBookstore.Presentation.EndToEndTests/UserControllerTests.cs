using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Bogus;
using FizzWare.NBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.UserFeatures;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Presentation.EndToEndTests;

public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly Faker _faker;
    private readonly string  _userId; 

    public UserControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        _userId = Guid.NewGuid().ToString();
        var adminUser = Builder<User>
            .CreateNew()
            .With(u => u.Id = _userId)
            .Build();
        
        using(var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            dbContext.Users.Add(adminUser);
            dbContext.SaveChanges();
        }

        var token = GenerateJwtAsync(adminUser, RoleName.Admin.ToString());

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetUserDataAsync_ReturnsUserData()
    {
        // Arrange
        const string url = "/api/users/";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedUserDto = JsonConvert.DeserializeObject<GetUserDto>(responseString)!;
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var userFromDb = dbContext.Users.FirstOrDefault(u => u.Id == _userId)!;
        
        Assert.Equal(userFromDb.FirstName, returnedUserDto.FirstName);
        Assert.Equal(userFromDb.LastName, returnedUserDto.LastName);

        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task IsAdmin_ReturnsBooleanIsUserHasAdminRole()
    {
        // Arrange
        const string url = "/api/users/is-admin";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedIsUserAdmin = JsonConvert.DeserializeObject<bool>(responseString);
        
        Assert.True(returnedIsUserAdmin);
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task RegisterUserAsync_CreatesNewUserInDb()
    {
        // Arrange
        var createUserDto = Builder<RegisterUserDto>
            .CreateNew()
            .With(u => u.Email = _faker.Random.Word() + "@email.com")
            .With(u => u.Password = "#Test123")
            .With(u => u.ConfirmPassword = "#Test123")
            .Build();

        var content = new StringContent(
            JsonConvert.SerializeObject(createUserDto), Encoding.UTF8, "application/json");
        
        const string url = "/api/users/register-user";

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var createdUser = dbContext.Users.FirstOrDefault(u => u.Email == createUserDto.Email)!;
        
        Assert.Equal(createUserDto.FirstName, createdUser.FirstName);
        Assert.Equal(createUserDto.LastName, createdUser.LastName);

        await RemoveAllEntitiesAsync(dbContext);
    }

    [Fact]
    public async Task RegisterAdminAsync_CreatesNewUserInDbWithAdminRole()
    {
        // Arrange
        var createUserDto = Builder<RegisterUserDto>
            .CreateNew()
            .With(u => u.Email = _faker.Random.Word() + "@email.com")
            .With(u => u.Password = "#Test123")
            .With(u => u.ConfirmPassword = "#Test123")
            .Build();

        var content = new StringContent(
            JsonConvert.SerializeObject(createUserDto), Encoding.UTF8, "application/json");
        
        const string url = "/api/users/register-admin";

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var createdUser = dbContext.Users.FirstOrDefault(u => u.Email == createUserDto.Email)!;

        Assert.Equal(createUserDto.FirstName, createdUser.FirstName);
        Assert.Equal(createUserDto.LastName, createdUser.LastName);

        await RemoveAllEntitiesAsync(dbContext);
    }
    
    private static async Task RemoveAllEntitiesAsync(DataContext dbContext)
    {
        foreach (var entity in dbContext.Users)
            dbContext.Users.Remove(entity);
        
        foreach (var entity in dbContext.Books)
            dbContext.Books.Remove(entity);

        await dbContext.SaveChangesAsync();
    }

    private static string GenerateJwtAsync(User user, string roleName)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, roleName),
        };

        var authSigningKey = new SymmetricSecurityKey("this-is-just-as-strong-key-as-possible"u8.ToArray());

        var token = new JwtSecurityToken(
            issuer: "OnlineBookstore-backend",
            audience: "user",
            expires: DateTime.UtcNow.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }
}