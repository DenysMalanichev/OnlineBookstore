using OnlineBookstore.Features.OrderFeatures;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IOrderService
{
    Task<GetOrderDto> GetUsersActiveOrderAsync(string userId);

    Task<IEnumerable<GetOrderDto>> GetUsersOrdersAsync(string userId);

    Task CloseUsersOrderAsync(CreateOrderDto createOrderDto);
}