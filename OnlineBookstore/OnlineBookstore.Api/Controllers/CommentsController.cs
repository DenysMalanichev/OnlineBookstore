using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Implementation;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Extentions;
using OnlineBookstore.Features.CommentFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
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
        
        await _commentService.AddCommentAsync(createCommentDto, userId);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetCommentByIdAsync(int commentId)
    {
        var comment = await _commentService.GetCommentByIdAsync(commentId);

        return Ok(comment);
    }

    [HttpGet("comments-by-book")]
    public async Task<IActionResult> GetCommentsByBookIdAsync(int bookId)
    {
        var comments = await _commentService.GetCommentsByBookIdAsync(bookId);

        return Ok(comments);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> DeleteCommentAsync(int commentId)
    {
        await _commentService.DeleteCommentAsync(commentId);

        return Ok();
    }
}