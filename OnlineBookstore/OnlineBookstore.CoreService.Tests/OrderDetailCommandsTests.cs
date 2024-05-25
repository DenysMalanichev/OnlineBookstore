using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.OrderDetails;
using OnlineBookstore.Application.OrderDetails.Create;
using OnlineBookstore.Application.OrderDetails.Delete;
using OnlineBookstore.Application.OrderDetails.Dtos;
using OnlineBookstore.Application.OrderDetails.Update;
using OnlineBookstore.Application.Orders.Dtos;
using OnlineBookstore.Application.Orders.GetUserActiveOrder;

namespace OnlineBookstore.CoreService.Tests;

public class OrderDetailCommandsTests
{
     private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IOrderDetailCommandRepository> _orderDetailRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;

    public OrderDetailCommandsTests()
    {
        _unitOfWorkMock.Setup(uow => uow.OrderDetailRepository)
            .Returns(_orderDetailRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateOrderDetailCommand, OrderDetail>().ReverseMap();
            cfg.CreateMap<GetOrderDetailDto, OrderDetail>().ReverseMap();
            cfg.CreateMap<UpdateOrderDetailCommand, OrderDetail>().ReverseMap();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddOrderDetailAsync_ShouldCallAddAndSaveRepoMethods()
    {
        // Arrange
        var addOrderDetailDto = Builder<CreateOrderDetailCommand>.CreateNew().Build();
        
        var orderDetail = _mapper.Map<OrderDetail>(addOrderDetailDto);
        var order = Builder<GetOrderDto>.CreateNew().Build();

        _orderDetailRepoMock.Setup(odr => odr.AddAsync(orderDetail))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<OrderDetail>(addOrderDetailDto))
            .Returns(orderDetail);
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActiveOrderQuery>(), CancellationToken.None))
            .ReturnsAsync(order);

        var orderDetailServ = new CreateOrderDetailCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, mediatorMock.Object);
        
        // Act
        await orderDetailServ.Handle(addOrderDetailDto, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _orderDetailRepoMock.Verify(gr => gr.AddAsync(orderDetail), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderDetailAsync_ShouldCallSaveRepoMethods_IfExists()
    {
        // Arrange
        const int orderDetailId = 100;
        var updateOrderDetailDto = Builder<UpdateOrderDetailCommand>.CreateNew()
            .With(od => od.Id = orderDetailId)
            .Build();
        var orderDetail = _mapper.Map<OrderDetail>(updateOrderDetailDto);
        
        _mapperMock.Setup(m => m.Map<OrderDetail>(updateOrderDetailDto))
            .Returns(orderDetail);

        var orderDetailServ = new UpdateOrderDetailCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await orderDetailServ.Handle(updateOrderDetailDto, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task DeleteOrderDetailAsync_ShouldCallSaveRepoMethods_IfExists()
    {
        // Arrange
        const int orderDetailId = 100;
        var orderDetail = Builder<OrderDetail>.CreateNew()
            .With(od => od.Id = orderDetailId)
            .Build();

        var orderDetailServ = new DeleteOrderDetailCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        // Act
        await orderDetailServ.Handle(
            new DeleteOrderDetailCommand { OrderDetailId = orderDetailId }, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
        _orderDetailRepoMock.Verify(uow => uow.DeleteAsync(orderDetail.Id), Times.Once);
    }
}