using System.Net.Http.Json;
using Recommendations.Abstractions.Entities;

namespace Recommendations.Tests.IntegrationTests;

[Collection("MongoDB Collection")]
public class RecommendationsControllerTests : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _mongoFixture;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    // Sample user ID for testing
    private const string TestUserId = "user1234";

    public RecommendationsControllerTests(MongoDbFixture mongoFixture)
    {
        _mongoFixture = mongoFixture;
        _factory = new CustomWebApplicationFactory<Program>(mongoFixture);
        _client = _factory.CreateClient();

        // Seed the database with test data
        SeedDatabase().GetAwaiter().GetResult();
    }

    private async Task SeedDatabase()
    {
        // Clear existing data
        await _mongoFixture.Database.DropCollectionAsync("BookPortraits");
        await _mongoFixture.Database.DropCollectionAsync("UserPortraits");

        // Create collections
        await _mongoFixture.Database.CreateCollectionAsync("BookPortraits");
        await _mongoFixture.Database.CreateCollectionAsync("UserPortraits");

        var bookPortraits = _mongoFixture.Database.GetCollection<BookPortrait>("BookPortraits");
        var userPortraits = _mongoFixture.Database.GetCollection<UserPortrait>("UserPortraits");

        // Seed books
        var books = new List<BookPortrait>
            {
                new BookPortrait
                {
                    BookId = 1,
                    Language = "English",
                    GenreIds = new List<int> { 1, 2 },
                    AuthorId = 101,
                    Rating = 4.5,
                    PurchaseNumber = 1000,
                    IsPaperback = true
                },
                new BookPortrait
                {
                    BookId = 2,
                    Language = "English",
                    GenreIds = new List<int> { 1, 3 },
                    AuthorId = 102,
                    Rating = 4.2,
                    PurchaseNumber = 800,
                    IsPaperback = false
                },
                new BookPortrait
                {
                    BookId = 3,
                    Language = "Spanish",
                    GenreIds = new List<int> { 2, 4 },
                    AuthorId = 103,
                    Rating = 4.8,
                    PurchaseNumber = 1200,
                    IsPaperback = true
                },
                new BookPortrait
                {
                    BookId = 4,
                    Language = "English",
                    GenreIds = new List<int> { 3, 5 },
                    AuthorId = 101,
                    Rating = 4.0,
                    PurchaseNumber = 600,
                    IsPaperback = false
                },
                new BookPortrait
                {
                    BookId = 5,
                    Language = "French",
                    GenreIds = new List<int> { 6 },
                    AuthorId = 104,
                    Rating = 3.9,
                    PurchaseNumber = 400,
                    IsPaperback = true
                }
            };

        await bookPortraits.InsertManyAsync(books);

        // Seed user
        var user = new UserPortrait
        {
            UserId = TestUserId,
            PreferedLanguages = new List<string> { "English" },
            PreferedGenreIds = new List<int> { 1, 2, 3 },
            PreferedAuthoreIds = new List<int> { 101 },
            IsPaperbackPrefered = true,
            PurchasedBooks = new List<int> { 1 }
        };

        await userPortraits.InsertOneAsync(user);
    }

    [Fact]
    public async Task GeneratePersonalRecommendations_ReturnsCorrectBooks()
    {
        // Act
        var response = await _client.GetAsync($"/api/Recommendations?userId={TestUserId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var recommendations = await response.Content.ReadFromJsonAsync<List<int>>();

        Assert.NotNull(recommendations);
        Assert.NotEmpty(recommendations);

        // Book 1 should not be in recommendations as it's already purchased
        Assert.DoesNotContain(1, recommendations);
        // Other should be ordered by most important criteria
        Assert.Equivalent(new int[] { 2, 4, 3, 5 }, recommendations);
    }


    [Fact]
    public async Task GeneratePersonalRecommendations_WithNonExistentUser_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/Recommendations?userId=nonexistentuser");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GeneratePersonalRecommendations_PaginationWorks()
    {
        // Add more books to test pagination
        var bookPortraits = _mongoFixture.Database.GetCollection<BookPortrait>("BookPortraits");

        var additionalBooks = new List<BookPortrait>();
        for (int i = 6; i <= 15; i++)
        {
            additionalBooks.Add(new BookPortrait
            {
                BookId = i*10,
                Language = "English",
                GenreIds = new List<int> { 1 },
                AuthorId = 101,
                Rating = 3.5,
                PurchaseNumber = 100,
                IsPaperback = true
            });
        }

        await bookPortraits.InsertManyAsync(additionalBooks);

        // Act - Get first page with 5 items
        var response1 = await _client.GetAsync($"/api/Recommendations?userId={TestUserId}&pageSize=5&pageNumber=1");

        // Assert
        response1.EnsureSuccessStatusCode();
        var page1 = await response1.Content.ReadFromJsonAsync<List<int>>();

        Assert.NotNull(page1);
        Assert.Equal(5, page1.Count);

        // Act - Get second page
        var response2 = await _client.GetAsync($"/api/Recommendations?userId={TestUserId}&pageSize=5&pageNumber=2");

        // Assert
        response2.EnsureSuccessStatusCode();
        var page2 = await response2.Content.ReadFromJsonAsync<List<int>>();

        Assert.NotNull(page2);
        Assert.Equal(5, page2.Count);

        // Make sure pages don't overlap
        foreach (var id in page1)
        {
            Assert.DoesNotContain(id, page2);
        }
    }
}
