using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Bogus;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.OrderFeatures;
using OnlineBookstore.Features.PublisherFeatures;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Presentation.EndToEndTests;

public class PublisherControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly Faker _faker;
    private readonly string  _userId; 

    public PublisherControllerTests(CustomWebApplicationFactory<Program> factory)
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
    public async Task GetPublisherByIdAsync_ReturnsPublisherDto()
    {
        // Arrange
        var publisher = Builder<Publisher>
            .CreateNew()
            .Build();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await dbContext.Publishers.AddAsync(publisher);
        await dbContext.SaveChangesAsync();
        
        var url = $"/api/publishers/{publisher.Id}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedPublisherDto = JsonConvert.DeserializeObject<GetPublisherDto>(responseString)!;
        
        Assert.Equal(publisher.CompanyName, returnedPublisherDto.CompanyName);
        Assert.Equal(publisher.ContactName, returnedPublisherDto.ContactName);

        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task GetAllPublishersAsync_ReturnListOfAllPublishersDto()
    {
        // Arrange
        var publishers = Builder<Publisher>
            .CreateListOfSize(10)
            .Build();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await dbContext.Publishers.AddRangeAsync(publishers);
        await dbContext.SaveChangesAsync();
        
        const string url = "/api/publishers";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedPublisherDto = JsonConvert.DeserializeObject<IEnumerable<GetPublisherDto>>(responseString)!
            .ToList();
        
        Assert.Equal(publishers.Count, returnedPublisherDto.Count);

        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task CreatePublisherAsync_CreatesNewPublisherAndReturnsDto()
    {
        // Arrange
        var createPublisherDto = Builder<CreatePublisherDto>
            .CreateNew()
            .With(p => p.Phone = _faker.Phone.PhoneNumber())
            .Build();
        
        var content = new StringContent(
            JsonConvert.SerializeObject(createPublisherDto), Encoding.UTF8, "application/json");
        
        const string url = "/api/publishers";

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var createdPublisher = dbContext.Publishers.FirstOrDefault(p =>
            p.ContactName == createPublisherDto.ContactName)!;
        
        Assert.Equal(createPublisherDto.ContactName, createdPublisher.ContactName);
        Assert.Equal(createPublisherDto.CompanyName, createdPublisher.CompanyName);

        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task UpdatePublisherAsync_UpdatesPublisherDataInDb()
    {
        // Arrange
        var publisher = Builder<Publisher>
            .CreateNew()
            .With(p => p.Phone = _faker.Phone.PhoneNumber())
            .Build();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await dbContext.Publishers.AddAsync(publisher);
        await dbContext.SaveChangesAsync();

        dbContext.Entry(publisher).State = EntityState.Detached;

        var updatePublisherDto = Builder<UpdatePublisherDto>
            .CreateNew()
            .With(p => p.Phone = _faker.Phone.PhoneNumber())
            .Build();
        
        var content = new StringContent(
            JsonConvert.SerializeObject(updatePublisherDto), Encoding.UTF8, "application/json");
        
        const string url = "/api/publishers";

        // Act
        var response = await _client.PutAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();

        var updatedPublisher = dbContext.Publishers.FirstOrDefault(p => p.Id == publisher.Id)!;
        
        Assert.Equal(updatePublisherDto.ContactName, updatedPublisher.ContactName);
        Assert.Equal(updatePublisherDto.CompanyName, updatedPublisher.CompanyName);

        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task DeletePublisherAsync_RemovesPublisherDataFromDb()
    {
        // Arrange
        var publisher = Builder<Publisher>
            .CreateNew()
            .With(p => p.Phone = _faker.Phone.PhoneNumber())
            .Build();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await dbContext.Publishers.AddAsync(publisher);
        await dbContext.SaveChangesAsync();

        dbContext.Entry(publisher).State = EntityState.Detached;
        
        var url = $"/api/publishers?publisherId={publisher.Id}";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();

        Assert.Null(dbContext.Publishers.FirstOrDefault(p => p.Id == publisher.Id));

        await RemoveAllEntitiesAsync(dbContext);
    }
    
    private static async Task RemoveAllEntitiesAsync(DataContext dbContext)
    {
        foreach (var entity in dbContext.Publishers)
            dbContext.Publishers.Remove(entity);
        
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