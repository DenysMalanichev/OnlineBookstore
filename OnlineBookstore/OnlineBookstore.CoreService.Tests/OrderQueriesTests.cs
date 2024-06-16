using OnlineBookstore.Application.Orders;
using OnlineBookstore.Application.Orders.Dtos;
using OnlineBookstore.Application.Orders.GetUserActiveOrder;
using OnlineBookstore.Application.Orders.GetUserOrders;
using OnlineBookstore.Domain.Constants;

namespace OnlineBookstore.CoreService.Tests;

public class OrderQueriesTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IOrderQueryRepository> _orderRepoMock = new();
    private readonly IMapper _mapper;

    public OrderQueriesTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GetOrderDto, Order>().ReverseMap();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task GetUsersActiveOrderAsync_ReturnsOrder_IdExists()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var orders = Builder<Order>.CreateListOfSize(5)
            .All()
            .With(o => o.UserId = userId)
            .With(o => o.OrderStatus = OrderStatus.Open)
            .Build();
        var orderDto = _mapper.Map<GetOrderDto>(orders[0]);

        _orderRepoMock.Setup(odr => odr.GetUserOrdersAsync(userId))
            .ReturnsAsync(orders);
        _mapperMock.Setup(m => m.Map<GetOrderDto>(orders[0]))
            .Returns(orderDto);

        var orderServ = new GetUserActiveOrderQueryHandler(_orderRepoMock.Object, _mapperMock.Object, null!);
        
        // Act
        var returnedOrderDto = await orderServ.Handle(
            new GetUserActiveOrderQuery { UserId = userId }, CancellationToken.None);
        
        // Assert
        returnedOrderDto.Should().BeEquivalentTo(orderDto);
    }

    [Fact]
    public async Task GetUsersOrdersAsync_ReturnsUserOrders()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var orders = Builder<Order>.CreateListOfSize(5)
            .All()
            .With(o => o.UserId = userId)
            .With(o => o.OrderStatus = OrderStatus.Open)
            .Build();
        var orderDto = _mapper.Map<IEnumerable<GetOrderDto>>(orders);

        _orderRepoMock.Setup(odr => odr.GetUserOrdersAsync(userId))
            .ReturnsAsync(orders);
        var getOrderDto = orderDto as GetOrderDto[] ?? orderDto.ToArray();
        _mapperMock.Setup(m => m.Map<IEnumerable<GetOrderDto>>(orders))
            .Returns(getOrderDto);

        var orderServ = new GetUserOrdersQueryHandler(_orderRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedOrderDto = await orderServ.Handle(
            new GetUserOrdersQuery { UserId = userId }, CancellationToken.None);
        
        // Assert
        returnedOrderDto.Should().BeEquivalentTo(getOrderDto);
    }
}