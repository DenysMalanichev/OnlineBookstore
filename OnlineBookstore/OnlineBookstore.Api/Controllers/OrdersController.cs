using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Extentions;
using OnlineBookstore.Features.OrderFeatures;
using OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetUsersActiveOrderAsync()
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        var orderDto = await _orderService.GetUsersActiveOrderAsync(userId);

        return Ok(orderDto);
    }

    [HttpGet("user-orders-history")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetUsersOrdersAsync()
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        var ordersDtos = await _orderService.GetUsersOrdersAsync(userId);

        return Ok(ordersDtos);
    }

    [HttpPost("ship-users-order")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CloseUsersOrderAsync(CreateOrderDto createOrderDto)
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        await _orderService.CloseUsersOrderAsync(createOrderDto, userId);

        return Ok();
    }

    [HttpPost("add-order-detail")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddOrderDetailAsync(AddOrderDetailDto addOrderDetailDto)
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        await _orderDetailService.AddOrderDetailAsync(addOrderDetailDto, userId);

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