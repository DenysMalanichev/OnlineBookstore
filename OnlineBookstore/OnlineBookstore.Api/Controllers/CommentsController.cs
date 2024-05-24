using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Comments.Create;
using OnlineBookstore.Application.Comments.Dtos;
using OnlineBookstore.Application.Comments.GetByBook;
using OnlineBookstore.Application.Comments.GetById;
using OnlineBookstore.Extentions;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateCommentAsync(CreateCommentDto createCommentDto)
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        
        await _mediator.Send(new CreateCommentCommand
        {
            Body = createCommentDto.Body,
            BookId = createCommentDto.BookId,
            BookRating = createCommentDto.BookRating,
            Title = createCommentDto.Title,
            UserId = userId
        });

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetCommentByIdAsync(int commentId)
    {
        var comment = await _mediator.Send(new GetCommentByIdQuery { CommentId = commentId});

        return Ok(comment);
    }

    [HttpGet("comments-by-book")]
    public async Task<IActionResult> GetCommentsByBookIdAsync(int bookId)
    {
        var comments = await _mediator.Send(new GetCommentsByBookQuery {BookId = bookId});

        return Ok(comments);
    }
}