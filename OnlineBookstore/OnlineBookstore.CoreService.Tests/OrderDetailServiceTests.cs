using OnlineBookstore.Application.OrderDetails;
using OnlineBookstore.Application.OrderDetails.Dtos;
using OnlineBookstore.Application.OrderDetails.GetById;

namespace OnlineBookstore.CoreService.Tests;

public class OrderDetailServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IOrderDetailQueryRepository> _orderDetailRepoMock = new();
    private readonly IMapper _mapper;

    public OrderDetailServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GetOrderDetailDto, OrderDetail>().ReverseMap();
        });

        _mapper = config.CreateMapper();
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

        var orderDetailServ = new GetOrderDetailByIdQueryHandler(_orderDetailRepoMock.Object, _mapperMock.Object);
        
        // Act
        var returnedOrderDetailDto = await orderDetailServ.Handle(
            new GetOrderDetailByIdQuery { OrderDetailId = orderDetailId }, CancellationToken.None);
        
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

        var orderDetailServ = new GetOrderDetailByIdQueryHandler(_orderDetailRepoMock.Object, _mapperMock.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () => 
            await orderDetailServ.Handle(
                new GetOrderDetailByIdQuery { OrderDetailId = orderDetailId }, CancellationToken.None));
    }
}