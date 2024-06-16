using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Bogus;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.AuthorFeatures;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Presentation.EndToEndTests;

public class AuthorControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly Faker _faker;

    public AuthorControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        var adminUser = Builder<User>.CreateNew().Build();
        var token = GenerateJwtAsync(adminUser, RoleName.Admin.ToString());

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task CreateAuthorAsync_CreateNewAuthorInDb()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        var createNewAuthorDto = Builder<CreateAuthorDto>
            .CreateNew()
            .With(a => a.Email = _faker.Random.Word() + "@email.com")
            .Build();

        var content = new StringContent(
            JsonConvert.SerializeObject(createNewAuthorDto), Encoding.UTF8, "application/json");
        const string url = "/api/author";

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();

        Assert.NotNull(await dbContext.Authors.FirstOrDefaultAsync(a =>
            a.FirstName == createNewAuthorDto.FirstName && a.LastName == createNewAuthorDto.LastName));
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task UpdateAuthorAsync_UpdatesAuthorDataInDb()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        const int authorId = 100; 
        var author = Builder<Author>
            .CreateNew()
            .With(a => a.Id = authorId)
            .Build();

        await dbContext.Authors.AddAsync(author);
        await dbContext.SaveChangesAsync();
        
        dbContext.Entry(author).State = EntityState.Detached;

        var updateAuthorDto = Builder<UpdateAuthorDto>
            .CreateNew()
            .With(a => a.Email = _faker.Random.Word() + "@email.com")
            .With(a => a.Id = authorId)
            .Build();

        var content = new StringContent(
            JsonConvert.SerializeObject(updateAuthorDto), Encoding.UTF8, "application/json");
        const string url = "/api/author";

        // Act
        var response = await _client.PutAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();
        
        Assert.Equal(updateAuthorDto.Email,
            (await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == authorId))!.Email);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task GetAuthorAsync_returnsFoundAuthor()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        const int authorId = 101; 
        var author = Builder<Author>
            .CreateNew()
            .With(a => a.Id = authorId)
            .Build();

        await dbContext.Authors.AddAsync(author);
        await dbContext.SaveChangesAsync();
        
        var url = $"/api/author/{authorId}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedAuthorDto = JsonConvert.DeserializeObject<GetAuthorDto>(responseString);
        returnedAuthorDto.Should().BeEquivalentTo(author);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task GetAllAuthorsAsync_ReturnsFoundAuthor()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        var startedAuthorId = 101; 
        var authors = Builder<Author>
            .CreateListOfSize(10)
            .All()
            .With(a => a.Id = ++startedAuthorId)
            .Build();

        await dbContext.SaveChangesAsync();
        
        await dbContext.Authors.AddRangeAsync(authors);
        await dbContext.SaveChangesAsync();
        
        const string url = "/api/author";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedAuthorsDto = JsonConvert.DeserializeObject<IEnumerable<GetAuthorDto>>(responseString);
        returnedAuthorsDto.Should().BeEquivalentTo(authors);

        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task DeleteAuthorAsync_RemovesAuthorFromDb()
    {
        // Arrange
        var authorId = 105; 
        var author = Builder<Author>
            .CreateNew()
            .With(a => a.Id = ++authorId)
            .Build();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        await dbContext.SaveChangesAsync();
        
        await dbContext.Authors.AddRangeAsync(author);
        await dbContext.SaveChangesAsync();
        
        var url = $"/api/author?authorId={authorId}";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        await dbContext.Entry(author).ReloadAsync();
        
        Assert.Null(await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == authorId));
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

    private static async Task RemoveAllEntitiesAsync(DataContext dbContext)
    {
        foreach (var entity in dbContext.Books)
            dbContext.Books.Remove(entity);
        
        foreach (var entity in dbContext.Authors)
            dbContext.Authors.Remove(entity);
        
        foreach (var entity in dbContext.Publishers)
            dbContext.Publishers.Remove(entity);
        
        await dbContext.SaveChangesAsync();
    }
}