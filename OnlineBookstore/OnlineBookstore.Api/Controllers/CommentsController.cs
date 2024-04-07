using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
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
    public async Task<IActionResult> CreateCommentAsync(CreateCommentDto createCommentDto)
    {
        await _commentService.AddCommentAsync(createCommentDto);

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
}