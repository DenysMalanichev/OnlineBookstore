using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Features.GenreFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> CreateGenreAsync(CreateGenreDto createGenreDto)
    {
        await _genreService.AddGenreAsync(createGenreDto);

        return Ok();
    }
    
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> UpdateGenreAsync(UpdateGenreDto updateGenreDto)
    {
        await _genreService.UpdateGenreAsync(updateGenreDto);

        return Ok();
    }
    
    [HttpGet("{genreId:int}")]
    public async Task<IActionResult> GetGenreAsync(int genreId)
    {
        var genreDto = await _genreService.GetGenreByIdAsync(genreId);

        return Ok(genreDto);
    }
    
    [HttpGet("by-book/{bookId:int}")]
    public async Task<IActionResult> GetGenresByBookAsync(int bookId)
    {
        var genreDto = await _genreService.GetGenresByBookAsync(bookId);

        return Ok(genreDto);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllGenresAsync()
    {
        var genresDto = await _genreService.GetAllGenresAsync();

        return Ok(genresDto);
    }
    
    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> DeleteGenreAsync(int genreId)
    {
        await _genreService.DeleteGenreAsync(genreId);

        return Ok();
    }
}