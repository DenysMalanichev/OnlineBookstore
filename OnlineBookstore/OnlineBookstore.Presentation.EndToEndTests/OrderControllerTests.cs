using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Bogus;
using FizzWare.NBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineBookstore.Application.OrderDetails.Dtos;
using OnlineBookstore.Application.Orders.Dtos;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Persistence.Context;

namespace OnlineBookstore.Presentation.EndToEndTests;

public class OrderControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly string  _userId; 

    public OrderControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        _userId = Guid.NewGuid().ToString();
        var adminUser = Builder<User>
            .CreateNew()
            .With(u => u.Id = _userId)
            .Build();
        
        using(var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            dbContext.Users.Add(adminUser);
            dbContext.SaveChanges();
        }

        var token = GenerateJwtAsync(adminUser, RoleName.Admin.ToString());

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetUsersActiveOrderAsync_CreatesNewOrderAndReturnsItsDto_IfDoesntExist()
    {
        // Arrange
        const string url = "/api/orders/users-active-order";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedOrderDto = JsonConvert.DeserializeObject<GetOrderDto>(responseString)!;
        
        Assert.Equal(OrderStatus.Open.ToString(), returnedOrderDto.Status);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task GetUserOrdersAsync_ReturnsListOfUserOrdersHistory()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var orders = Builder<Order>
            .CreateListOfSize(10)
            .All()
            .With(o => o.UserId = _userId)
            .With(o => o.OrderStatus = OrderStatus.Closed)
            .Build();

        await dbContext.Orders.AddRangeAsync(orders);
        await dbContext.SaveChangesAsync();

        const string url = "/api/orders/user-orders-history";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedOrderDto = JsonConvert.DeserializeObject<IEnumerable<GetOrderDto>>(responseString)!
            .ToList();
        
        Assert.Equal(OrderStatus.Closed.ToString(), returnedOrderDto[0].Status);
        Assert.Equal(10, returnedOrderDto.Count);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
        
    [Fact]
    public async Task CloseUsersOrderAsync_ChangesUserActiveOrderStatusToClosed()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var order = Builder<Order>
            .CreateNew()
            .With(o => o.UserId = _userId)
            .With(o => o.OrderStatus = OrderStatus.Open)
            .Build();

        await dbContext.Orders.AddRangeAsync(order);
        await dbContext.SaveChangesAsync();

        var shipOrderDto = Builder<CreateOrderDto>.CreateNew().Build();
        var content = new StringContent(
            JsonConvert.SerializeObject(shipOrderDto), Encoding.UTF8, "application/json");

        const string url = "/api/orders/ship-users-order";

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();
        
        Assert.Equal(OrderStatus.Closed, dbContext.Orders.FirstOrDefault(o => o.Id == order.Id)!.OrderStatus);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task AddOrderDetailAsync_AddsDetailToUserOpenOrder()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var order = Builder<Order>
            .CreateNew()
            .With(o => o.UserId = _userId)
            .With(o => o.OrderStatus = OrderStatus.Open)
            .Build();
        var addOrderDetailDto = Builder<AddOrderDetailDto>.CreateNew().Build();

        await dbContext.Orders.AddRangeAsync(order);
        await dbContext.SaveChangesAsync();
        
        var content = new StringContent(
            JsonConvert.SerializeObject(addOrderDetailDto), Encoding.UTF8, "application/json");

        const string url = "/api/orders/add-order-detail";

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var foundOrder = dbContext.Orders.FirstOrDefault(o => o.Id == order.Id)!;
        
        Assert.Equal(addOrderDetailDto.OrderId, foundOrder.Id);
        Assert.Equal(addOrderDetailDto.BookId, foundOrder.OrderDetails[0].BookId);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task GetOrderDetailAsync_ReturnsOrderDetailDto()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var order = Builder<Order>
            .CreateNew()
            .With(o => o.UserId = _userId)
            .With(o => o.OrderStatus = OrderStatus.Open)
            .Build();
        var orderDetail = Builder<OrderDetail>
            .CreateNew()
            .With(od => od.OrderId = order.Id)
            .Build();

        await dbContext.Orders.AddAsync(order);
        await dbContext.OrderDetails.AddAsync(orderDetail);
        await dbContext.SaveChangesAsync();

        var url = $"/api/orders/get-order-detail?orderDetailId={orderDetail.Id}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var returnedOrderDetailDto = JsonConvert.DeserializeObject<GetOrderDetailDto>(responseString)!;
        
        Assert.Equal(orderDetail.BookId, returnedOrderDetailDto.BookId);
        Assert.Equal(orderDetail.Quantity, returnedOrderDetailDto.Quantity);
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    [Fact]
    public async Task UpdateOrderDetailAsync_UpdatesOrderDetailDateInDb()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var order = Builder<Order>
            .CreateNew()
            .With(o => o.UserId = _userId)
            .With(o => o.OrderStatus = OrderStatus.Open)
            .Build();
        var orderDetail = Builder<OrderDetail>
            .CreateNew()
            .With(od => od.OrderId = order.Id)
            .Build();

        var updateOrderDetailDto = Builder<UpdateOrderDetailDto>
            .CreateNew()
            .Build();

        await dbContext.Orders.AddAsync(order);
        await dbContext.OrderDetails.AddAsync(orderDetail);
        await dbContext.SaveChangesAsync();

        var content = new StringContent(
            JsonConvert.SerializeObject(updateOrderDetailDto), Encoding.UTF8, "application/json");
        
        const string url = "/api/orders/update-order-detail";

        // Act
        var response = await _client.PutAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode();
        
        await RemoveAllEntitiesAsync(dbContext);
    }

    [Fact]
    public async Task DeleteOrderDetailAsync_RemovesOrderDetailDateFromDb()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var order = Builder<Order>
            .CreateNew()
            .With(o => o.UserId = _userId)
            .With(o => o.OrderStatus = OrderStatus.Open)
            .Build();
        var orderDetail = Builder<OrderDetail>
            .CreateNew()
            .With(od => od.OrderId = order.Id)
            .Build();

        await dbContext.Orders.AddAsync(order);
        await dbContext.OrderDetails.AddAsync(orderDetail);
        await dbContext.SaveChangesAsync();
        
        var url = $"/api/orders/delete-order-detail?orderDetailId={orderDetail.Id}";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        
        Assert.Null(dbContext.OrderDetails.FirstOrDefault(od => od.Id == orderDetail.Id));
        
        await RemoveAllEntitiesAsync(dbContext);
    }
    
    private static async Task RemoveAllEntitiesAsync(DataContext dbContext)
    {
        foreach (var entity in dbContext.Orders)
            dbContext.Orders.Remove(entity);
        
        foreach (var entity in dbContext.Books)
            dbContext.Books.Remove(entity);

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