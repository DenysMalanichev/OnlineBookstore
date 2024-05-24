using Microsoft.EntityFrameworkCore.Storage;

namespace OnlineBookstore.Persistence.Tests;

public class OrderDetailRepositoryTests
{
    private readonly DbContextOptions<DataContext> _dataContextOptions;
    private readonly InMemoryDatabaseRoot _databaseRoot;

    public OrderDetailRepositoryTests()
    {
        _databaseRoot = new InMemoryDatabaseRoot();
        _dataContextOptions = CreateNewContextOptions();
    }
    
    private DataContext CreateContext() => new(_dataContextOptions);
    
    [Fact]
    public async Task GetAsync_ReturnsGenreBuId()
    {
        // Arrange
        const int orderDetailId = 100;
        var orderDetail = Builder<OrderDetail>
            .CreateNew()
            .With(g => g.Id = orderDetailId)
            .Build();
        
        await using var context = CreateContext();

        await context.OrderDetails.AddAsync(orderDetail);
        await context.SaveChangesAsync();

        var orderDetailRepository = new OrderDetailQueryRepository(context);

        // Act
        var result = await orderDetailRepository.GetByIdAsync(orderDetailId);

        // Assert
        result.Should().BeEquivalentTo(orderDetail);
    }
    
    private DbContextOptions<DataContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString(), _databaseRoot)
            .Options;
    }
}