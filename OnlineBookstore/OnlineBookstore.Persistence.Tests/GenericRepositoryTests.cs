namespace OnlineBookstore.Persistence.Tests;

public class GenericRepositoryTests
{
    private readonly DataContext _dataContext;
    private readonly IAuthorRepository _authorRepository;

    public GenericRepositoryTests()
    {
        var options = CreateNewContextOptions();
        _dataContext = new DataContext(options);
        _authorRepository = new AuthorRepository(_dataContext);
    }
    
    [Fact]
    public async Task CreateAsync_SavesToDatabase()
    {
        // Arrange
        var entity = Builder<Author>.CreateNew().Build();

        // Act
        await _authorRepository.AddAsync(entity);
        var addedBook = await _dataContext.Authors.FirstOrDefaultAsync(b => b.Id == entity.Id);

        // Assert
        addedBook.Should().BeEquivalentTo(entity);
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
    
    [Fact]
    public async Task AddRangeAsync_AddsListOfEntities()
    {
        // Arrange
        var entities = Builder<Author>.CreateListOfSize(10).Build();

        // Act
        await _authorRepository.AddRangeAsync(entities);

        // Assert
        _dataContext.Authors.Should().BeEquivalentTo(entities);
    }
    
    [Fact]
    public async Task DeleteAsync_RemovesEntityFromDb()
    {
        // Arrange
        var entity = Builder<Author>.CreateNew().Build();
        await _dataContext.Authors.AddRangeAsync(entity);
        await _dataContext.SaveChangesAsync();

        // Act
        await _authorRepository.DeleteAsync(entity);

        // Assert
        Assert.Null(await _dataContext.Authors.FirstOrDefaultAsync(a => a.Id == entity.Id));
    }
    
    [Fact]
    public async Task UpdateAsync_UpdatesDataIsDb()
    {
        // Arrange
        var entity = Builder<Author>.CreateNew().Build();
        await _dataContext.Authors.AddAsync(entity);
        await _dataContext.SaveChangesAsync();

        var newEntity = Builder<Author>.CreateNew()
            .With(e => e.Id = entity.Id)
            .Build();
        
        DetachAllEntities(_dataContext);
        
        // Act
        await _authorRepository.UpdateAsync(newEntity);

        // Assert
        _dataContext.Authors.AsNoTracking().FirstOrDefault(a => a.Id == entity.Id).Should().BeEquivalentTo(newEntity);
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