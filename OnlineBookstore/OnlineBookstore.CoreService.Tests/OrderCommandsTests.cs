using MediatR;
using OnlineBookstore.Application.Common;
using OnlineBookstore.Application.Orders;
using OnlineBookstore.Application.Orders.CloseUsersOrder;
using OnlineBookstore.Application.Orders.Create;
using OnlineBookstore.Application.Orders.Dtos;
using OnlineBookstore.Application.Orders.GetUserActiveOrder;
using OnlineBookstore.Domain.Constants;

namespace OnlineBookstore.CoreService.Tests;

public class OrderCommandsTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IOrderCommandRepository> _orderRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;

    public OrderCommandsTests()
    {
        _unitOfWorkMock.Setup(uow => uow.OrderRepository)
            .Returns(_orderRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.CommitAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateOrderCommand, Order>().ReverseMap();
        });

        _mapper = config.CreateMapper();
    }
    
    
    [Fact]
    public async Task CloseUsersOrderAsync_ReturnsUserOrders()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var closeOrderCommand = Builder<CloseUsersOrderCommand>.CreateNew().Build();
        var orderDto = Builder<GetOrderDto>
            .CreateNew()
            .Build();
        var closeOrderData = Builder<CloseOrderData>
            .CreateNew()
            .With(cod => cod.OrderId = orderDto.Id)
            .With(cod => cod.ShipAddress = closeOrderCommand.ShipAddress)
            .With(cod => cod.ShipCity = closeOrderCommand.ShipCity)
            .Build();

        _mapperMock.Setup(m => m.Map<CloseOrderData>(closeOrderCommand))
            .Returns(closeOrderData);
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(
                It.IsAny<GetUserActiveOrderQuery>(), CancellationToken.None))
            .ReturnsAsync(orderDto);

        var orderServ = new CloseUsersOrderCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, mediatorMock.Object);
        
        // Act
        await orderServ.Handle(closeOrderCommand, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(CancellationToken.None), Times.Once);
    }
}