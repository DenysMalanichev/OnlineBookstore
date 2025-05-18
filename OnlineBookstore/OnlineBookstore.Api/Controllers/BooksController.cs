using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Features.BookFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> CreateBookAsync(CreateBookDto createBookDto)
    {
        await _bookService.AddBookAsync(createBookDto);

        return Ok();
    }
    
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> UpdateBookAsync(UpdateBookDto updateBookDto)
    {
        await _bookService.UpdateBookAsync(updateBookDto);

        return Ok();
    }

    [HttpGet("recommendations")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetRecommendationsAsync(int? page, int itemsOnPage = 10)
    {
        var identifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if(identifier is null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(identifier);
        var result = await _bookService.GetRecommendationsAsync(userId, page, itemsOnPage);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetBookAsync(int bookId)
    {
        var bookDto = await _bookService.GetBookByIdAsync(bookId);

        return Ok(bookDto);
    }
    
    [HttpGet("get-filtered-books")]
    public async Task<IActionResult> GetFilteredBooksAsync([FromQuery] GetFilteredBooksDto filteredBooksDto)
    {
        var pagedBooksResult = await _bookService.GetBooksUsingFiltersAsync(filteredBooksDto);

        return Ok(pagedBooksResult);
    }
    
    [HttpGet("by-author")]
    public IActionResult GetBooksByAuthor(int authorId, int? page, int itemsOnPage = 10)
    {
        var pagedBooksResult = _bookService.GetBooksByAuthor(authorId, page, itemsOnPage);

        return Ok(pagedBooksResult);
    }
    
    [HttpGet("by-publisher")]
    public IActionResult GetBooksBuPublisher(int publisherId, int? page, int itemsOnPage = 10)
    {
        var pagedBooksResult = _bookService.GetBooksByPublisher(publisherId, page, itemsOnPage);

        return Ok(pagedBooksResult);
    }

    [HttpGet("avg-rating/{bookId:int}")]
    public IActionResult GetAvgBookRating(int bookId)
    {
        var avgRating = _bookService.CountAvgRatingOfBook(bookId);

        return Ok(avgRating);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> DeleteBookAsync(int bookId)
    {
        await _bookService.DeleteBookAsync(bookId);

        return Ok();
    }
}