using Microsoft.EntityFrameworkCore.Storage;

namespace OnlineBookstore.Persistence.Tests;

public class PublisherRepositoryTests
{
    private readonly DbContextOptions<DataContext> _dataContextOptions;
    private readonly InMemoryDatabaseRoot _databaseRoot;

    public PublisherRepositoryTests()
    {
        _databaseRoot = new InMemoryDatabaseRoot();
        _dataContextOptions = CreateNewContextOptions();
    }
    
    private DataContext CreateContext() => new(_dataContextOptions);
    
    [Fact]
    public async Task GetAsync_ReturnsGenreBuId()
    {
        // Arrange
        const int publisherId = 100;
        var publisher = Builder<Publisher>
            .CreateNew()
            .With(g => g.Id = publisherId)
            .Build();
        
        await using var context = CreateContext();

        await context.Publishers.AddAsync(publisher);
        await context.SaveChangesAsync();

        var publisherRepository = new PublisherQueryRepository(context);

        // Act
        var result = await publisherRepository.GetByIdAsync(publisherId);

        // Assert
        result.Should().BeEquivalentTo(publisher);
    }
    
    private DbContextOptions<DataContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString(), _databaseRoot)
            .Options;
    }
}