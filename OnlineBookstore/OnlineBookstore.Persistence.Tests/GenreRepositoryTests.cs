using Microsoft.EntityFrameworkCore.Storage;

namespace OnlineBookstore.Persistence.Tests;

public class GenreRepositoryTests
{
    private readonly DbContextOptions<DataContext> _dataContextOptions;
    private readonly InMemoryDatabaseRoot _databaseRoot;

    public GenreRepositoryTests()
    {
        _databaseRoot = new InMemoryDatabaseRoot();
        _dataContextOptions = CreateNewContextOptions();
    }
    
    private DataContext CreateContext() => new(_dataContextOptions);
    
    [Fact]
    public async Task GetAsync_ReturnsGenreBuId()
    {
        // Arrange
        const int genreId = 100;
        var genre = Builder<Genre>
            .CreateNew()
            .With(g => g.Id = genreId)
            .Build();
        
        await using var context = CreateContext();

        await context.Genres.AddAsync(genre);
        await context.SaveChangesAsync();

        var genreRepository = new GenreRepository(context);

        // Act
        var result = await genreRepository.GetByIdAsync(genreId);

        // Assert
        result.Should().BeEquivalentTo(genre);
    }
    
    private DbContextOptions<DataContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString(), _databaseRoot)
            .Options;
    }
}