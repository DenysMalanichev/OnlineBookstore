using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Features.GenreFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenreAsync(CreateGenreDto createGenreDto)
    {
        await _genreService.AddGenreAsync(createGenreDto);

        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateGenreAsync(UpdateGenreDto updateGenreDto)
    {
        await _genreService.UpdateGenreAsync(updateGenreDto);

        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetGenreAsync(int genreId)
    {
        var genreDto = await _genreService.GetGenreByIdAsync(genreId);

        return Ok(genreDto);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteGenreAsync(int genreId)
    {
        await _genreService.DeleteGenreAsync(genreId);

        return Ok();
    }
}