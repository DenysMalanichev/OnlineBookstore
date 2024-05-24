using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Author.Create;
using OnlineBookstore.Application.Author.GetAll;
using OnlineBookstore.Application.Author.GetAuthorById;
using OnlineBookstore.Application.Author.Update;
using OnlineBookstore.Domain.Constants;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthorController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> CreateAuthorAsync(CreateAuthorCommand createAuthorCommand)
    {
        await _mediator.Send(createAuthorCommand);

        return Ok();
    }
    
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> UpdateAuthorAsync(UpdateAuthorCommand updateAuthorCommand)
    {
        await _mediator.Send(updateAuthorCommand);

        return Ok();
    }
    
    [HttpGet("{authorId:int}")]
    public async Task<IActionResult> GetAuthorAsync(int authorId)
    {
        var authorDto = await _mediator.Send(new GetAuthorByIdQuery { AuthorId = authorId });

        return Ok(authorDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAuthorsAsync()
    {
        var authorDto = await _mediator.Send(new GetAllAuthorsQuery());

        return Ok(authorDto);
    }
    
    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> DeleteAuthorAsync(int authorId)
    {
        await _mediator.Send(authorId);

        return Ok();
    }
}