using Microsoft.EntityFrameworkCore.Storage;

namespace OnlineBookstore.Persistence.Tests;

public class CommentCommandRepositoryTests
{
    private readonly DbContextOptions<DataContext> _dataContextOptions;
    private readonly InMemoryDatabaseRoot _databaseRoot;

    public CommentCommandRepositoryTests()
    {
        _databaseRoot = new InMemoryDatabaseRoot();
        _dataContextOptions = CreateNewContextOptions();
    }
    
    private DataContext CreateContext() => new(_dataContextOptions);
    
    [Fact]
    public async Task GetCommentsByBookIdAsync_ReturnsBookEntityAsPagedResult()
    {
        // Arrange
        const int bookId = 100;

        var book = Builder<Book>
            .CreateNew()
            .With(b => b.Id = bookId)
            .With(b => b.Publisher = new Publisher { CompanyName = "as", ContactName = "as" })
            .With(b => b.Genres = Array.Empty<Genre>())
            .With(b => b.Author = new Author { FirstName = "ad", LastName = "ad"})
            .Build();
        
        await using var context = CreateContext();
        await context.Books.AddAsync(book);
        await context.SaveChangesAsync();

        var comments = Builder<Comment>
            .CreateListOfSize(10)
            .All()
            .With(c => c.BookId = bookId)
            .With(c => c.Book = book)
            .Build();

        await context.Comments.AddRangeAsync(comments);
        await context.SaveChangesAsync();

        var commentRepository = new CommentQueryRepository(context);

        // Act
        var foundComments = await commentRepository.GetCommentsByBookIdAsync(bookId);

        // Assert
        foundComments.Should().BeEquivalentTo(comments.ToList());
    }
        
    [Fact]
    public async Task IsUserWroteCommentForThisBook_ReturnsTrueIfUserCommentedBook()
    {
        // Arrange
        const int bookId = 100;
        var userId = Guid.NewGuid().ToString();
        await using var context = CreateContext();

        var comments = Builder<Comment>
            .CreateNew()
            .With(c => c.UserId = userId)
            .With(c => c.BookId = bookId)
            .Build();

        await context.Comments.AddRangeAsync(comments);
        await context.SaveChangesAsync();

        var commentRepository = new CommentCommandRepository(context);

        // Act
        var result = commentRepository.IsUserWroteCommentForThisBook(userId, bookId);

        // Assert
        Assert.True(result);
    }
    
    private DbContextOptions<DataContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString(), _databaseRoot)
            .Options;
    }
}