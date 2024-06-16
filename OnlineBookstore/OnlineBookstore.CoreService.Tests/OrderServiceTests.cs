using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Features.OrderFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class OrderServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;

    public OrderServiceTests()
    {
        _unitOfWorkMock.Setup(uow => uow.OrderRepository)
            .Returns(_orderRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync())
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateOrderDto, Order>().ReverseMap();
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

        var orderServ = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object, null!);
        
        // Act
        var returnedOrderDto = await orderServ.GetUsersActiveOrderAsync(userId);
        
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

        var orderServ = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object, null!);
        
        // Act
        var returnedOrderDto = await orderServ.GetUsersOrdersAsync(userId);
        
        // Assert
        returnedOrderDto.Should().BeEquivalentTo(getOrderDto);
    }

    [Fact]
    public async Task CloseUsersOrderAsync_ReturnsUserOrders()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var createOrderDto = Builder<CreateOrderDto>.CreateNew().Build();
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

        var orderServ = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object, null!);
        
        // Act
        await orderServ.CloseUsersOrderAsync(createOrderDto, userId);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CloseUsersOrderAsync_ThrowEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var createOrderDto = Builder<CreateOrderDto>.CreateNew().Build();

        _orderRepoMock.Setup(odr => odr.GetUserOrdersAsync(userId))
            .ReturnsAsync(new List<Order>());

        var orderServ = new OrderService(_unitOfWorkMock.Object, _mapperMock.Object, null!);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await orderServ.CloseUsersOrderAsync(createOrderDto, userId));
    }
}