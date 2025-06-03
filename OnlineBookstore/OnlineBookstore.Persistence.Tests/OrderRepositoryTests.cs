using Microsoft.EntityFrameworkCore.Storage;

namespace OnlineBookstore.Persistence.Tests;

public class OrderRepositoryTests
{
    private readonly DbContextOptions<DataContext> _dataContextOptions;
    private readonly InMemoryDatabaseRoot _databaseRoot;

    public OrderRepositoryTests()
    {
        _databaseRoot = new InMemoryDatabaseRoot();
        _dataContextOptions = CreateNewContextOptions();
    }
    
    private DataContext CreateContext() => new(_dataContextOptions);
    
    [Fact]
    public async Task GetUserOrdersAsync_ReturnsTrueIfUserCommentedBook()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = new User { Id = userId, FirstName = "a", LastName = "as" };
        
        await using var context = CreateContext();

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        
        var orders = Builder<Order>
            .CreateListOfSize(10)
            .All()
            .With(o => o.OrderDetails = new List<OrderDetail>())
            .With(o => o.UserId = userId)
            .With(o => o.User = user)
            .Build();

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();

        var orderRepository = new OrderRepository(context);

        // Act
        var result = await orderRepository.GetUserOrdersAsync(userId);

        // Assert
        result.Should().BeEquivalentTo(orders);
    }
    
    private DbContextOptions<DataContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString(), _databaseRoot)
            .Options;
    }
}