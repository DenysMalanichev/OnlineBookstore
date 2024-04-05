using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Features.OrderFeatures;
using OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IOrderDetailService _orderDetailService;

    public OrdersController(IOrderService orderService, IOrderDetailService orderDetailService)
    {
        _orderService = orderService;
        _orderDetailService = orderDetailService;
    }

    [HttpGet("users-active-order")]
    public async Task<IActionResult> GetUsersActiveOrderAsync(string userId)
    {
        var orderDto = await _orderService.GetUsersActiveOrderAsync(userId);

        return Ok(orderDto);
    }

    [HttpGet("user-orders-history")]
    public async Task<IActionResult> GetUsersOrdersAsync(string userId)
    {
        var ordersDtos = await _orderService.GetUsersOrdersAsync(userId);

        return Ok(ordersDtos);
    }

    [HttpPost("ship-users-order")]
    public async Task<IActionResult> CloseUsersOrderAsync(CreateOrderDto createOrderDto)
    {
        await _orderService.CloseUsersOrderAsync(createOrderDto);

        return Ok();
    }

    [HttpPost("add-order-detail")]
    public async Task<IActionResult> AddOrderDetailAsync(AddOrderDetailDto addOrderDetailDto)
    {
        await _orderDetailService.AddOrderDetailAsync(addOrderDetailDto);

        return Ok();
    }

    [HttpGet("get-order-detail")]
    public async Task<IActionResult> GetOrderDetailAsync(int orderDetailId)
    {
        var orderDetailDto = await _orderDetailService.GetOrderDetailByIdAsync(orderDetailId);

        return Ok(orderDetailDto);
    }

    [HttpPut("update-order-detail")]
    public async Task<IActionResult> UpdateOrderDetailAsync(UpdateOrderDetailDto updateOrderDetailDto)
    {
        await _orderDetailService.UpdateOrderDetailAsync(updateOrderDetailDto);

        return Ok();
    }

    [HttpDelete("delete-order-detail")]
    public async Task<IActionResult> DeleteOrderDetailAsync(int orderDetailId)
    {
        await _orderDetailService.DeleteOrderDetailAsync(orderDetailId);

        return Ok();
    }
}