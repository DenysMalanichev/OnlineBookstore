using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Genres.Create;
using OnlineBookstore.Application.Genres.Delete;
using OnlineBookstore.Application.Genres.GetAll;
using OnlineBookstore.Application.Genres.GetByBook;
using OnlineBookstore.Application.Genres.GetById;
using OnlineBookstore.Application.Genres.Update;
using OnlineBookstore.Domain.Constants;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IMediator _mediator;

    public GenresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> CreateGenreAsync(CreateGenreCommand createGenreCommand)
    {
        await _mediator.Send(createGenreCommand);

        return Ok();
    }
    
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> UpdateGenreAsync(UpdateGenreCommand updateGenreCommand)
    {
        await _mediator.Send(updateGenreCommand);

        return Ok();
    }
    
    [HttpGet("{genreId:int}")]
    public async Task<IActionResult> GetGenreAsync(int genreId)
    {
        var genreDto = await _mediator.Send(new GetGenreByIdQuery { GenreId = genreId });

        return Ok(genreDto);
    }
    
    [HttpGet("by-book/{bookId:int}")]
    public async Task<IActionResult> GetGenresByBookAsync(int bookId)
    {
        var genreDto = await _mediator.Send(new GetGenresByBookQuery { BookId = bookId });

        return Ok(genreDto);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllGenresAsync()
    {
        var genresDto = await _mediator.Send(new GetAllGenresQuery());

        return Ok(genresDto);
    }
    
    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> DeleteGenreAsync(int genreId)
    {
        await _mediator.Send(new DeleteGenreCommand { GenreId = genreId });

        return Ok();
    }
}