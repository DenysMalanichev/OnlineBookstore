using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Books.Create;
using OnlineBookstore.Application.Books.Delete;
using OnlineBookstore.Application.Books.GetAvgBookRating;
using OnlineBookstore.Application.Books.GetBooksByAuthor;
using OnlineBookstore.Application.Books.GetBooksByPublisher;
using OnlineBookstore.Application.Books.GetBooksUsingFilters;
using OnlineBookstore.Application.Books.GetById;
using OnlineBookstore.Application.Books.Update;
using OnlineBookstore.Domain.Constants;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> CreateBookAsync(CreateBookCommand createBookCommand)
    {
        await _mediator.Send(createBookCommand);

        return Ok();
    }
    
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> UpdateBookAsync(UpdateBookCommand updateBookCommand)
    {
        await _mediator.Send(updateBookCommand);

        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBookAsync(int bookId)
    {
        var bookDto = await _mediator.Send(new GetBookByIdQuery { BookId = bookId });

        return Ok(bookDto);
    }
    
    [HttpGet("get-filtered-books")]
    public async Task<IActionResult> GetFilteredBooksAsync([FromQuery] GetFilteredBooksQuery filteredBooksQuery)
    {
        var pagedBooksResult = await _mediator.Send(filteredBooksQuery);

        return Ok(pagedBooksResult);
    }
    
    [HttpGet("by-author")]
    public IActionResult GetBooksByAuthor(int authorId, int? page, int itemsOnPage = 10)
    {
        var pagedBooksResult = _mediator.Send(new GetBooksByAuthorQuery
        {
            AuthorId = authorId,
            Page = page,
            ItemsOnPage = itemsOnPage
        });

        return Ok(pagedBooksResult);
    }
    
    [HttpGet("by-publisher")]
    public IActionResult GetBooksBuPublisher(int publisherId, int? page, int itemsOnPage = 10)
    {
        var pagedBooksResult = _mediator.Send(new GetBooksByPublisherQuery
        {
            PublisherId = publisherId,
            Page = page,
            ItemsOnPage = itemsOnPage
        });

        return Ok(pagedBooksResult);
    }

    [HttpGet("avg-rating/{bookId:int}")]
    public IActionResult GetAvgBookRating(int bookId)
    {
        var avgRating = _mediator.Send(new GetAvgBookRatingQuery { BookId = bookId });

        return Ok(avgRating);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> DeleteBookAsync(int bookId)
    {
        await _mediator.Send(new DeleteBookCommand { BookId = bookId });

        return Ok();
    }
}