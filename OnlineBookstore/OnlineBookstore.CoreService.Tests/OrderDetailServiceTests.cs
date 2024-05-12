using OnlineBookstore.Features.OrderFeatures;
using OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;

namespace OnlineBookstore.CoreService.Tests;

public class OrderDetailServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IOrderDetailRepository> _orderDetailRepoMock = new();
    private readonly Mock<IOrderService> _orderServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;

    public OrderDetailServiceTests()
    {
        _unitOfWorkMock.Setup(uow => uow.OrderDetailRepository)
            .Returns(_orderDetailRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync())
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<AddOrderDetailDto, OrderDetail>().ReverseMap();
            cfg.CreateMap<GetOrderDetailDto, OrderDetail>().ReverseMap();
            cfg.CreateMap<UpdateOrderDetailDto, OrderDetail>().ReverseMap();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddOrderDetailAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var addOrderDetailDto = Builder<AddOrderDetailDto>.CreateNew().Build();
        var orderDetail = _mapper.Map<OrderDetail>(addOrderDetailDto);
        var order = Builder<GetOrderDto>.CreateNew().Build();

        _orderDetailRepoMock.Setup(odr => odr.AddAsync(orderDetail))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<OrderDetail>(addOrderDetailDto))
            .Returns(orderDetail);
        _orderServiceMock.Setup(os => os.GetUsersActiveOrderAsync(userId))
            .ReturnsAsync(order);

        var orderDetailServ = new OrderDetailService(_unitOfWorkMock.Object, _mapperMock.Object, _orderServiceMock.Object);
        
        // Act
        await orderDetailServ.AddOrderDetailAsync(addOrderDetailDto, userId);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _orderDetailRepoMock.Verify(gr => gr.AddAsync(orderDetail), Times.Once);
    }
    
    [Fact]
    public async Task GetOrderDetailByIdAsync_ShouldReturnOrderDetailDto_IfExists()
    {
        // Arrange
        const int orderDetailId = 100;
        var orderDetail = Builder<OrderDetail>.CreateNew()
            .With(od => od.Id = orderDetailId)
            .Build();
        var orderDetailDto = _mapper.Map<GetOrderDetailDto>(orderDetail);
        
        _orderDetailRepoMock.Setup(odr => odr.GetByIdAsync(orderDetailId, false))!
            .ReturnsAsync(orderDetail);
        _mapperMock.Setup(m => m.Map<GetOrderDetailDto>(orderDetail))
            .Returns(orderDetailDto);

        var orderDetailServ = new OrderDetailService(_unitOfWorkMock.Object, _mapperMock.Object, _orderServiceMock.Object);
        
        // Act
        var returnedOrderDetailDto = await orderDetailServ.GetOrderDetailByIdAsync(orderDetailId);
        
        // Assert
        returnedOrderDetailDto.Should().BeEquivalentTo(orderDetailDto);
    }
    
    [Fact]
    public async Task GetOrderDetailByIdAsync_ShouldThrowEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        const int orderDetailId = 100;
        
        _orderDetailRepoMock.Setup(odr => odr.GetByIdAsync(orderDetailId, false))!
            .ReturnsAsync((OrderDetail)null!);

        var orderDetailServ = new OrderDetailService(_unitOfWorkMock.Object, _mapperMock.Object, _orderServiceMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await orderDetailServ.GetOrderDetailByIdAsync(orderDetailId));
    }

    [Fact]
    public async Task UpdateOrderDetailAsync_ShouldThrowEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        const int orderDetailId = 100;
        
        _orderDetailRepoMock.Setup(odr => odr.GetByIdAsync(orderDetailId, false))!
            .ReturnsAsync((OrderDetail)null!);

        var orderDetailServ = new OrderDetailService(_unitOfWorkMock.Object, _mapperMock.Object, _orderServiceMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await orderDetailServ.UpdateOrderDetailAsync(new UpdateOrderDetailDto {Id = orderDetailId}));
    }

    [Fact]
    public async Task UpdateOrderDetailAsync_ShouldCallSaveRepoMethods_IfExists()
    {
        // Arrange
        const int orderDetailId = 100;
        var updateOrderDetailDto = Builder<UpdateOrderDetailDto>.CreateNew()
            .With(od => od.Id = orderDetailId)
            .Build();
        var orderDetail = _mapper.Map<OrderDetail>(updateOrderDetailDto);
        
        _orderDetailRepoMock.Setup(odr => odr.GetByIdAsync(orderDetailId, false))!
            .ReturnsAsync(orderDetail);
        _mapperMock.Setup(m => m.Map<OrderDetail>(updateOrderDetailDto))
            .Returns(orderDetail);

        var orderDetailServ = new OrderDetailService(_unitOfWorkMock.Object, _mapperMock.Object, _orderServiceMock.Object);
        
        // Act
        await orderDetailServ.UpdateOrderDetailAsync(updateOrderDetailDto);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task DeleteOrderDetailAsync_ShouldThrowEntityNotFoundException_IfDoesntExist()
    {
        // Arrange
        const int orderDetailId = 100;
        
        _orderDetailRepoMock.Setup(odr => odr.GetByIdAsync(orderDetailId, false))!
            .ReturnsAsync((OrderDetail)null!);

        var orderDetailServ = new OrderDetailService(_unitOfWorkMock.Object, _mapperMock.Object, _orderServiceMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await orderDetailServ.DeleteOrderDetailAsync(orderDetailId));
    }
    
    [Fact]
    public async Task DeleteOrderDetailAsync_ShouldCallSaveRepoMethods_IfExists()
    {
        // Arrange
        const int orderDetailId = 100;
        var orderDetail = Builder<OrderDetail>.CreateNew()
            .With(od => od.Id = orderDetailId)
            .Build();
        
        _orderDetailRepoMock.Setup(odr => odr.GetByIdAsync(orderDetailId, false))!
            .ReturnsAsync(orderDetail);

        var orderDetailServ = new OrderDetailService(_unitOfWorkMock.Object, _mapperMock.Object, _orderServiceMock.Object);
        
        // Act
        await orderDetailServ.DeleteOrderDetailAsync(orderDetailId);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
        _orderDetailRepoMock.Verify(uow => uow.DeleteAsync(orderDetail), Times.Once);
    }
}