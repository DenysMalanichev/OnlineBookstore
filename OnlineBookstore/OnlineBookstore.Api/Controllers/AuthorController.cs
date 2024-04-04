using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Features.AuthorFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
    {
        await _authorService.AddAuthorAsync(createAuthorDto);

        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto)
    {
        await _authorService.UpdateAuthorAsync(updateAuthorDto);

        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAuthorAsync(int authorId)
    {
        var bookDto = await _authorService.GetAuthorByIdAsync(authorId);

        return Ok(bookDto);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteAuthorAsync(int authorId)
    {
        await _authorService.DeleteAuthorAsync(authorId);

        return Ok();
    }
}