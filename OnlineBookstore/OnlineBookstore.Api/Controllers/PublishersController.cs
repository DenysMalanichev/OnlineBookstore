using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Publishers.Create;
using OnlineBookstore.Application.Publishers.Delete;
using OnlineBookstore.Application.Publishers.GetAll;
using OnlineBookstore.Application.Publishers.GetById;
using OnlineBookstore.Application.Publishers.Update;
using OnlineBookstore.Domain.Constants;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublishersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublishersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{publisherId:int}")]
    public async Task<IActionResult> GetPublisherByIdAsync(int publisherId)
    {
        var publisher = await _mediator.Send(new GetPublisherByIdQuery { PublisherId = publisherId});

        return Ok(publisher);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllPublishersAsync()
    {
        var publishersDto = await _mediator.Send(new GetAllPublishersQuery());

        return Ok(publishersDto);
    }
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> CreatePublisherAsync(CreatePublisherCommand createPublisherCommand)
    {
        await _mediator.Send(createPublisherCommand);

        return Ok();
    }
    
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> UpdatePublisherAsync(UpdatePublisherCommand updatePublisherCommand)
    {
        await _mediator.Send(updatePublisherCommand);

        return Ok();
    }
    
    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> DeletePublisherAsync(int publisherId)
    {
        await _mediator.Send(new DeletePublisherCommand { PublisherId = publisherId });

        return Ok();
    }
}