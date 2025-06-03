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
using OnlineBookstore.Features.CommentFeatures;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Presentation.EndToEndTests;

public class CommentControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly Faker _faker;
    private readonly string  _userId; 

    public CommentControllerTests(CustomWebApplicationFactory<Program> factory)
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
        _faker = new Faker();
    }

    [Fact]
    public async Task GetCommentByIdAsync_ReturnsCommentDto()
    {
        // Arrange
        var comment = Builder<Comment>.CreateNew().Build();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await dbContext.Comments.AddAsync(comment);

        var url = $"/api/comments?commentId={comment.Id}";
        
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedCommentDto = JsonConvert.DeserializeObject<GetCommentDto>(responseString)!;
        
        Assert.Equal(comment.BookRating, returnedCommentDto.BookRating);
        Assert.Equal(comment.Body, returnedCommentDto.Body);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task CreateCommentAsync_CreatesNewCommentInDb()
    {
        // Arrange
        var createCommentDto = Builder<CreateCommentDto>.CreateNew().Build();

        var content = new StringContent(
            JsonConvert.SerializeObject(createCommentDto), Encoding.UTF8, "application/json");
        const string url = "/api/comments";
        
        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        Assert.Single(dbContext.Comments.Where(c => c.UserId == _userId).ToList());
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task GetCommentsByBookIdAsync_ReturnsCommentDto()
    {
        // Arrange
        const int bookId = 100;
        var userId = Guid.NewGuid().ToString();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        var book = Builder<Book>
            .CreateNew()
            .With(b => b.Id = bookId)
            .With(b => b.Author = new Author { FirstName = "as", LastName = "as" })
            .With(a => a.Publisher =  new Publisher { CompanyName = "c", ContactName = "n" })
            .With(a => a.GenreIds = Array.Empty<int>())
            .Build();
        var user = Builder<User>
            .CreateNew()
            .With(c => c.Id = userId) 
            .With(c => c.Email = _faker.Random.Word() + "@email.com") 
            .Build();
        var comments = Builder<Comment>
            .CreateListOfSize(10)
            .All()
            .With(c => c.BookId = bookId)
            .With(c => c.UserId = userId)
            .With(c => c.User = null!)
            .With(c => c.BookRating = _faker.Random.Int(1, 6))
            .Build();
        
        

        await RemoveAllEntitiesAsync(dbContext);
        
        await dbContext.Books.AddAsync(book);
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        
        await dbContext.Comments.AddRangeAsync(comments);
        await dbContext.SaveChangesAsync();
        
        var url = $"/api/comments/comments-by-book?bookId={bookId}";
        
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedCommentsDto = JsonConvert.DeserializeObject<IEnumerable<GetCommentDto>>(responseString)!
            .ToList();
        
        Assert.Equal(comments.Count, returnedCommentsDto.Count);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    private static async Task RemoveAllEntitiesAsync(DataContext dbContext)
    {
        foreach (var entity in dbContext.Comments)
        {
            dbContext.Comments.Remove(entity);
        }

        foreach (var entity in dbContext.Books)
        {
            dbContext.Books.Remove(entity);
        }
        
        foreach (var entity in dbContext.Authors)
        {
            dbContext.Authors.Remove(entity);
        }
        
        foreach (var entity in dbContext.Publishers)
        {
            dbContext.Publishers.Remove(entity);
        }

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