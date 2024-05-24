using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using FizzWare.NBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineBookstore.Application.Genres.Dtos;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace OnlineBookstore.Presentation.EndToEndTests;

public class GenreControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly string _userId; 

    public GenreControllerTests(CustomWebApplicationFactory<Program> factory)
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
        }

        var token = GenerateJwtAsync(adminUser, RoleName.Admin.ToString());

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CreateGenreAsync_CreatesNewGenreInDb()
    {
        // Arrange
        
        var createGenreDto = Builder<CreateGenreDto>
            .CreateNew()
            .Build();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await RemoveAllEntitiesAsync(dbContext);
        
        var content = new StringContent(
            JsonConvert.SerializeObject(createGenreDto), Encoding.UTF8, "application/json");
        const string url = "/api/genres";

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();
        
        Assert.Single(dbContext.Genres.Where(g => g.Name == createGenreDto.Name).ToList());
        
        await RemoveAllEntitiesAsync(dbContext);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ReturnsGenreDto()
    {
        // Arrange
        var genre = Builder<Genre>.CreateNew().Build();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await dbContext.Genres.AddAsync(genre);

        var url = $"/api/genres/{genre.Id}";
        
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedGenreDto = JsonConvert.DeserializeObject<GetGenreDto>(responseString)!;
        
        Assert.Equal(genre.Description, returnedGenreDto.Description);
        Assert.Equal(genre.Name, returnedGenreDto.Name);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task GetAllGenresAsync_ReturnsIEnumerableOfGenreDto()
    {
        // Arrange
        var genres = Builder<Genre>.CreateListOfSize(10).Build();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await RemoveAllEntitiesAsync(dbContext);
        
        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.SaveChangesAsync();

        const string url = "/api/genres";
        
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedGenreDto = JsonConvert.DeserializeObject<IEnumerable<GetGenreDto>>(responseString)!;
        
        Assert.Equal(genres.Count, returnedGenreDto.ToList().Count);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task UpdateGenreAsync_UpdatesGenreDataInDb()
    {
        // Arrange
        const int genreId = 100;
        var genre = Builder<Genre>
            .CreateNew()
            .With(g => g.Id = genreId)
            .Build();
        var updateGenreDto = Builder<UpdateGenreDto>
            .CreateNew()
            .With(g => g.Name = "NewName")
            .With(g => g.Description = "NewDesc")
            .With(g => g.Id = genreId)
            .Build();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await RemoveAllEntitiesAsync(dbContext);
        
        await dbContext.Genres.AddAsync(genre);
        await dbContext.SaveChangesAsync();
        dbContext.Entry(genre).State = EntityState.Detached;

        var content = new StringContent(
            JsonConvert.SerializeObject(updateGenreDto), Encoding.UTF8, "application/json");
        const string url = "/api/genres";
        
        // Act
        var response = await _client.PutAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();

        var foundGenre = dbContext.Genres.FirstOrDefault(g => g.Id == genreId)!;
        
        Assert.Equal(updateGenreDto.Name, foundGenre.Name);
        Assert.Equal(updateGenreDto.Description, foundGenre.Description);
        
        await RemoveAllEntitiesAsync(dbContext);
    }

    [Fact]
    public async Task DeleteGenreAsync_DeletesGenreFromDb()
    {
        // Arrange
        const int genreId = 100;
        var genre = Builder<Genre>
            .CreateNew()
            .With(g => g.Id = genreId)
            .Build();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await RemoveAllEntitiesAsync(dbContext);
        
        await dbContext.Genres.AddAsync(genre);
        await dbContext.SaveChangesAsync();
        dbContext.Entry(genre).State = EntityState.Detached;

        var url = $"/api/genres?genreId={genreId}";
        
        // Act
        var response = await _client.DeleteAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();

        Assert.Null(dbContext.Genres.FirstOrDefault(g => g.Id == genreId));
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task GetGenresByBookAsync_ReturnsListOfGenresByBook()
    {
        // Arrange
        var genres = Builder<Genre>
            .CreateListOfSize(10)
            .Build();
        var book = Builder<Book>
            .CreateNew()
            .With(b => b.Genres = genres)
            .With(b => b.Author = new Author { FirstName = "as", LastName = "as" })
            .With(a => a.Publisher =  new Publisher { CompanyName = "c", ContactName = "n" })
            .Build();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await RemoveAllEntitiesAsync(dbContext);
        
        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.Books.AddAsync(book);
        await dbContext.SaveChangesAsync();
        foreach (var genre in genres)
        {
            dbContext.Entry(genre).State = EntityState.Detached;
        }
        dbContext.Entry(book).State = EntityState.Detached;

        var url = $"/api/genres/by-book/{book.Id}";
        
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var returnedGenreDto = JsonConvert.DeserializeObject<IEnumerable<GetGenreDto>>(responseString)!;
        Assert.Equal(genres.Count, returnedGenreDto.ToList().Count);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    private static async Task RemoveAllEntitiesAsync(DataContext dbContext)
    {
        foreach (var entity in dbContext.Genres)
            dbContext.Genres.Remove(entity);
        
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