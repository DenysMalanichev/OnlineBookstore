using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.OrderDetails.Create;
using OnlineBookstore.Application.OrderDetails.Delete;
using OnlineBookstore.Application.OrderDetails.GetById;
using OnlineBookstore.Application.OrderDetails.Update;
using OnlineBookstore.Application.Orders.CloseUsersOrder;
using OnlineBookstore.Application.Orders.Create;
using OnlineBookstore.Application.Orders.GetUserActiveOrder;
using OnlineBookstore.Application.Orders.GetUserOrders;
using OnlineBookstore.Extentions;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
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
        var orderDto = await _mediator.Send(new GetUserActiveOrderQuery { UserId = userId });

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
        var ordersDtos = await _mediator.Send(new GetUserOrdersQuery { UserId = userId });

        return Ok(ordersDtos);
    }

    [HttpPost("ship-users-order")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CloseUsersOrderAsync(CloseUsersOrderCommand createOrderCommand)
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }

        createOrderCommand.UserId = userId;
        
        await _mediator.Send(createOrderCommand);

        return Ok();
    }

    [HttpPost("add-order-detail")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddOrderDetailAsync(CreateOrderDetailCommand addOrderDetailCommand)
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        
        addOrderDetailCommand.UserId = userId;
        
        await _mediator.Send(addOrderDetailCommand);

        return Ok();
    }

    [HttpGet("get-order-detail")]
    public async Task<IActionResult> GetOrderDetailAsync(int orderDetailId)
    {
        var orderDetailDto = await _mediator.Send(new GetOrderDetailByIdQuery { OrderDetailId = orderDetailId});

        return Ok(orderDetailDto);
    }

    [HttpGet("get-books-order-statistics/{bookId:int}")]
    public async Task<IActionResult> GetBooksOrderStatisticsAsync(int bookId)
    {
        var orderDetailDto = await _orderService.GetBooksOrderStatisticsAsync(bookId);

        return Ok(orderDetailDto);
    }


    [HttpPut("update-order-detail")]
    public async Task<IActionResult> UpdateOrderDetailAsync(UpdateOrderDetailCommand updateOrderDetailCommand)
    {
        await _mediator.Send(updateOrderDetailCommand);

        return Ok();
    }

    [HttpDelete("delete-order-detail")]
    public async Task<IActionResult> DeleteOrderDetailAsync(int orderDetailId)
    {
        await _mediator.Send(new DeleteOrderDetailCommand { OrderDetailId = orderDetailId });

        return Ok();
    }
}