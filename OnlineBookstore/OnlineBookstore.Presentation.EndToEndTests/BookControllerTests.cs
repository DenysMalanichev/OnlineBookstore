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
using OnlineBookstore.Application.Books.Create;
using OnlineBookstore.Application.Books.Dtos;
using OnlineBookstore.Application.Books.GetBooksUsingFilters;
using OnlineBookstore.Application.Books.Update;
using OnlineBookstore.Application.Common.Paging;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Presentation.EndToEndTests;

public class BookControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly Faker _faker;

    public BookControllerTests(CustomWebApplicationFactory<Program> factory)
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
    public async Task CreateBookAsync_CreateNewBookInDb()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await dbContext.Authors.AddAsync(Builder<Author>.CreateNew().Build());
        await dbContext.Publishers.AddAsync(Builder<Publisher>.CreateNew().Build());

        await dbContext.SaveChangesAsync();

        var createNewBookDto = Builder<CreateBookCommand>
            .CreateNew()
            .With(b => b.GenreIds = new List<int>())
            .Build();

        var content = new StringContent(
            JsonConvert.SerializeObject(createNewBookDto), Encoding.UTF8, "application/json");
        const string url = "/api/books";

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();

        Assert.NotNull(await dbContext.Books.AsNoTracking().FirstOrDefaultAsync(a =>
            a.Name == createNewBookDto.Name));

        await RemoveAllEntitiesAsync(dbContext);
    }

    [Fact]
    public async Task UpdateAuthorAsync_UpdatesBookDataInDb()
    {
        // Arrange
        const int bookId = 100;

        var book = Builder<Book>
            .CreateNew()
            .With(a => a.Id = bookId)
            .Build();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await dbContext.Authors.AddAsync(Builder<Author>.CreateNew().Build());
        await dbContext.Publishers.AddAsync(Builder<Publisher>.CreateNew().Build());

        await dbContext.Books.AddAsync(book);
        await dbContext.SaveChangesAsync();

        dbContext.Entry(book).State = EntityState.Detached;

        var updateBookDto = Builder<UpdateBookCommand>
            .CreateNew()
            .With(a => a.Name = _faker.Random.Word())
            .With(a => a.Id = bookId)
            .With(a => a.Price = 100)
            .With(a => a.GenreIds = new List<int>())
            .Build();

        var content = new StringContent(
            JsonConvert.SerializeObject(updateBookDto), Encoding.UTF8, "application/json");
        const string url = "/api/books";

        // Act
        var response = await _client.PutAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();

        Assert.Equal(updateBookDto.Name,
            (await dbContext.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bookId))!.Name);

        await RemoveAllEntitiesAsync(dbContext);
    }

    [Fact]
    public async Task GetBookAsync_UpdatesBooksDataInDb()
    {
        // Arrange
        const int bookId = 100;
        var book = Builder<Book>
            .CreateNew()
            .With(a => a.Id = bookId)
            .With(a => a.Author = new Author { FirstName = "as", LastName = "as" })
            .With(a => a.Publisher = new Publisher { CompanyName = "c", ContactName = "n" })
            .With(a => a.GenreIds = Array.Empty<int>())
            .Build();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await dbContext.Books.AddAsync(book);
        await dbContext.SaveChangesAsync();

        var url = $"/api/books?bookId={bookId}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var returnedBookDto = JsonConvert.DeserializeObject<GetBookDto>(responseString)!;
        Assert.Equal(book.Name, returnedBookDto.Name);
        Assert.Equal(book.Price, returnedBookDto.Price);

        await RemoveAllEntitiesAsync(dbContext);
    }

    [Fact]
    public async Task DeleteABookAsync_RemovesBookFromDb()
    {
        // Arrange
        const int bookId = 105;
        var book = Builder<Book>
            .CreateNew()
            .With(a => a.Id = bookId)
            .With(a => a.Author = new Author { FirstName = "as", LastName = "as" })
            .With(a => a.Publisher = new Publisher { CompanyName = "c", ContactName = "n" })
            .With(a => a.GenreIds = Array.Empty<int>())
            .Build();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await dbContext.Books.AddRangeAsync(book);
        await dbContext.SaveChangesAsync();

        var url = $"/api/books?bookId={bookId}";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();

        await dbContext.Entry(book).ReloadAsync();

        Assert.Null(await dbContext.Books.AsNoTracking().FirstOrDefaultAsync(a => a.Id == bookId));

        await RemoveAllEntitiesAsync(dbContext);
    }

    [Fact]
    public async Task GetAvgBookRating_ReturnsAvgRatingByComments()
    {
        // Arrange
        const int bookId = 105;
        const int expectedRating = 4;
        var comments = Builder<Comment>
            .CreateListOfSize(10)
            .All()
            .With(c => c.BookRating = expectedRating)
            .With(c => c.BookId = bookId)
            .Build();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await dbContext.Comments.AddRangeAsync(comments);
        await dbContext.SaveChangesAsync();

        var url = $"/api/books/avg-rating/{bookId}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedRating = JsonConvert.DeserializeObject<double>(responseString);
        Assert.Equal(expectedRating, returnedRating);

        await RemoveAllEntitiesAsync(dbContext);
    }

    [Fact]
    public async Task GetBooksByAuthorAsync_ReturnsListOfBooksWrittenByProvidedAuthorAsPagedResult()
    {
        // Arrange
        const int authorId = 105;
        const int page = 1;
        const int itemsOnPage = 10;

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var author = Builder<Author>.CreateNew().With(a => a.Id = authorId).Build();
        await dbContext.Authors.AddAsync(author);

        var books = Builder<Book>
            .CreateListOfSize(10)
            .All()
            .With(b => b.AuthorId = authorId)
            .With(b => b.Author = author)
            .With(a => a.Publisher = new Publisher { CompanyName = "c", ContactName = "n" })
            .With(a => a.GenreIds = Array.Empty<int>())
            .Build();

        await dbContext.Books.AddRangeAsync(books);
        await dbContext.SaveChangesAsync();

        var url = $"/api/books/by-author?authorId={authorId}&page={page}&itemsOnPage={itemsOnPage}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedPagedBooks = JsonConvert.DeserializeObject<GenericPagingDto<Book>>(responseString)!;
        Assert.Equal(books.Count, returnedPagedBooks.Entities.Count());
        Assert.Equal(books[0].Name, returnedPagedBooks.Entities.ToList()[0].Name);
        Assert.Equal(page, returnedPagedBooks.CurrentPage);

        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task GetBooksByPublisherAsync_ReturnsListOfBooksProvidedByPublisherAsPagedResult()
    {
        // Arrange
        const int publisherId = 105;
        const int page = 1;
        const int itemsOnPage = 10;

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var publisher = Builder<Publisher>.CreateNew().With(a => a.Id = publisherId).Build();
        await dbContext.Publishers.AddAsync(publisher);

        var books = Builder<Book>
            .CreateListOfSize(10)
            .All()
            .With(b => b.Author = new Author { FirstName = "as", LastName = "as" })
            .With(a => a.Publisher = publisher)
            .With(a => a.PublisherId = publisherId)
            .With(a => a.GenreIds = Array.Empty<int>())
            .Build();

        await dbContext.Books.AddRangeAsync(books);
        await dbContext.SaveChangesAsync();

        var url = $"/api/books/by-publisher?publisherId={publisherId}&page={page}&itemsOnPage={itemsOnPage}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedPagedBooks = JsonConvert.DeserializeObject<GenericPagingDto<Book>>(responseString)!;
        Assert.Equal(books.Count, returnedPagedBooks.Entities.Count());
        Assert.Equal(books[0].Name, returnedPagedBooks.Entities.ToList()[0].Name);
        Assert.Equal(page, returnedPagedBooks.CurrentPage);

        await RemoveAllEntitiesAsync(dbContext);
    }

    [Fact]
    public async Task GetFilteredBooksAsync_ReturnsListOfBooksFilteredByProvidedCriteria()
    {
        // Arrange
        const int bookId = 105;

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var book = Builder<Book>
            .CreateNew()
            .With(b => b.Id = bookId)
            .With(b => b.Author = new Author { FirstName = "as", LastName = "as" })
            .With(a => a.Publisher =  new Publisher { CompanyName = "c", ContactName = "n" })
            .With(a => a.GenreIds = Array.Empty<int>())
            .Build();

        await dbContext.Books.AddAsync(book);
        await dbContext.SaveChangesAsync();

        var bookSearchParams = new GetFilteredBooksQuery
        {
            Name = book.Name
        };
        
        var url = $"/api/books/get-filtered-books?Name={Uri.EscapeDataString(bookSearchParams.Name)}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedPagedBooks = JsonConvert.DeserializeObject<GenericPagingDto<Book>>(responseString)!;
        Assert.Equal(book.Name, returnedPagedBooks.Entities.ToList()[0].Name);
        Assert.Single(returnedPagedBooks.Entities);

        await RemoveAllEntitiesAsync(dbContext);
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