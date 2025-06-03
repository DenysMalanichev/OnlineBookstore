using OnlineBookstore.Application.Author;

namespace OnlineBookstore.Persistence.Tests;

public class GenericQueryRepositoryTests
{
    private readonly DataContext _dataContext;
    private readonly IAuthorQueryRepository _authorRepository;

    public GenericQueryRepositoryTests()
    {
        var options = CreateNewContextOptions();
        _dataContext = new DataContext(options);
        _authorRepository = new AuthorQueryRepository(_dataContext);
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_IfExists()
    {
        // Arrange
        var entity = Builder<Author>.CreateNew().Build();
        await _dataContext.Authors.AddAsync(entity);
        await _dataContext.SaveChangesAsync();

        // Act
        var foundBook = await _authorRepository.GetByIdAsync(entity.Id)!;

        // Assert
        foundBook.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsIEnumerableOfEntities()
    {
        // Arrange
        var entities = Builder<Author>.CreateListOfSize(10).Build();
        await _dataContext.Authors.AddRangeAsync(entities);
        await _dataContext.SaveChangesAsync();

        // Act
        var foundBooks = await _authorRepository.GetAllAsync();

        // Assert
        foundBooks.Should().BeEquivalentTo(entities);
    }
    
    private static DbContextOptions<DataContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }
    
    private static void DetachAllEntities(DbContext context)
    {
        var undetachedEntriesCopy = context.ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Detached)
            .ToList();

        foreach (var entry in undetachedEntriesCopy)
            entry.State = EntityState.Detached;
    }
}