using Bogus;
using LinqKit;
using Microsoft.EntityFrameworkCore.Storage;

namespace OnlineBookstore.Persistence.Tests;

public class BookQueryRepositoryTests
{
    private readonly DbContextOptions<DataContext> _dataContextOptions;
    private readonly InMemoryDatabaseRoot _databaseRoot;
    private readonly Faker _faker;

    public BookQueryRepositoryTests()
    {
        _databaseRoot = new InMemoryDatabaseRoot();
        _dataContextOptions = CreateNewContextOptions();
        _faker = new Faker();
    }
    
    private DataContext CreateContext() => new(_dataContextOptions);

    [Fact]
    public async Task GetBooksByAuthor_ReturnsBookEntityAsPagedResult()
    {
        // Arrange
        const int authorId = 100;
        const int page = 1;
        const int itemsOnPage = 5;
        const int totalBooks = 10;

        await using var context = CreateContext();
        var author = Builder<Author>.CreateNew()
            .With(p => p.Id = authorId)
            .Build();
        await context.Authors.AddAsync(author);
        await context.SaveChangesAsync();

        var books = Builder<Book>
            .CreateListOfSize(totalBooks)
            .All()
            .With(b => b.Publisher = new Publisher {CompanyName = "as", ContactName = "as"})
            .With(b => b.Genres = Array.Empty<Genre>())
            .With(b => b.AuthorId = authorId)
            .With(b => b.Author = null!)
            .Build();

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();
        
        var expectedBooks = books.Take(itemsOnPage).ToList();

        var bookRepository = new BookQueryRepository(context);

        // Act
        var foundBooks = bookRepository.GetBooksByAuthor(authorId, page, itemsOnPage);

        // Assert
        foundBooks.booksOnPage.Should().BeEquivalentTo(expectedBooks.ToList());
    }
    
    [Fact]
    public async Task GetBooksByPublisher_ReturnsBookEntityAsPagedResult()
    {
        // Arrange
        const int publisherId = 100;
        const int page = 1;
        const int itemsOnPage = 5;
        const int totalBooks = 10;

        await using var context = CreateContext();

        await context.Publishers.AddAsync(Builder<Publisher>.CreateNew().With(p => p.Id = publisherId).Build());
        await context.SaveChangesAsync();

        var books = Builder<Book>
            .CreateListOfSize(totalBooks)
            .All()
            .With(b => b.PublisherId = publisherId)
            .With(b => b.Publisher = null!)
            .With(b => b.Genres = Array.Empty<Genre>())
            .With(b => b.Author = new Author { FirstName = "ad", LastName = "ad"})
            .Build();

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();
        
        var expectedBooks = books.Take(itemsOnPage).ToList();

        var bookRepository = new BookQueryRepository(context);

        // Act
        var foundBooks = bookRepository.GetBooksByPublisher(publisherId, page, itemsOnPage);

        // Assert
        foundBooks.booksOnPage.Should().BeEquivalentTo(expectedBooks.ToList());
    }
    
    [Fact]
    public async Task CountAvgRatingForBook_ReturnsAvgBookRating()
    {
        // Arrange
        const int bookId = 100;

        var rand = new Random();
        
        var comments = Builder<Comment>
            .CreateListOfSize(10)
            .All()
            .With(c => c.BookRating = rand.Next(1, 6))
            .With(c => c.BookId = bookId)
            .Build();

        var expected = comments.Sum(c => c.BookRating) / 10.0;
    
        await using var context = CreateContext();
        await context.Comments.AddRangeAsync(comments);
        await context.SaveChangesAsync();

        var bookRepository = new BookQueryRepository(context);

        // Act
        var returnedAvgRating = bookRepository.CountAvgRatingForBook(bookId);

        // Assert
        Assert.Equal(expected, returnedAvgRating);
    }
    
    [Fact]
    public async Task CountAvgRatingForBook_ReturnsNull_IfNoRatingPresent()
    {
        // Arrange
        const int bookId = 100;
        
        await using var context = CreateContext();

        var bookRepository = new BookQueryRepository(context);

        // Act
        var returnedAvgRating = bookRepository.CountAvgRatingForBook(bookId);

        // Assert
        Assert.Null(returnedAvgRating);
    }
    
    [Fact]
    public async Task GetItemsByPredicate_ReturnsBookEntitiesAccordingToPredicate()
    {
        // Arrange
        const int publisherId = 100;
        const int totalBooks = 10;

        await using var context = CreateContext();

        await context.Publishers.AddAsync(Builder<Publisher>.CreateNew().With(p => p.Id = publisherId).Build());
        await context.SaveChangesAsync();

        var books = Builder<Book>
            .CreateListOfSize(totalBooks)
            .All()
            .With(b => b.PublisherId = publisherId)
            .With(b => b.Publisher = null!)
            .With(b => b.Genres = Array.Empty<Genre>())
            .With(b => b.Author = new Author { FirstName = "ad", LastName = "ad"})
            .With(b => b.Price = _faker.Random.Int(0, 20))
            .Build();

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();

        var predicate = PredicateBuilder.New<Book>(b => b.Price < 10);

        var expectedBooks = books.Where(b => b.Price < 10);
        
        var bookRepository = new BookQueryRepository(context);

        // Act
        var foundBooks = bookRepository.GetItemsByPredicate(predicate, false);

        // Assert
        foundBooks.Should().BeEquivalentTo(expectedBooks.ToList());
    }
    
    private DbContextOptions<DataContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString(), _databaseRoot)
            .Options;
    }
}