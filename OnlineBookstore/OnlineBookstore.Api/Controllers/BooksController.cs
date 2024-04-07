using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
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
    public async Task<IActionResult> CreateBookAsync(CreateBookDto createBookDto)
    {
        await _bookService.AddBookAsync(createBookDto);

        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateBookAsync(UpdateBookDto updateBookDto)
    {
        await _bookService.UpdateBookAsync(updateBookDto);

        return Ok();
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
    
    [HttpDelete]
    public async Task<IActionResult> DeleteBookAsync(int bookId)
    {
        await _bookService.DeleteBookAsync(bookId);

        return Ok();
    }
}