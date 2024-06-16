using OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;

namespace OnlineBookstore.Application.Services.Interfaces;

public interface IOrderDetailService
{
    Task AddOrderDetailAsync(AddOrderDetailDto addOrderDetailDto, string userId);
    
    Task<GetOrderDetailDto> GetOrderDetailByIdAsync(int orderDetailId);
    
    Task UpdateOrderDetailAsync(UpdateOrderDetailDto updateOrderDetailDto);
    
    Task DeleteOrderDetailAsync(int orderDetailId);
}