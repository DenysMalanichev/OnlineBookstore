using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Features.AuthorFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
    {
        await _authorService.AddAuthorAsync(createAuthorDto);

        return Ok();
    }
    
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto)
    {
        await _authorService.UpdateAuthorAsync(updateAuthorDto);

        return Ok();
    }
    
    [HttpGet("{authorId:int}")]
    public async Task<IActionResult> GetAuthorAsync(int authorId)
    {
        var authorDto = await _authorService.GetAuthorByIdAsync(authorId);

        return Ok(authorDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAuthorsAsync()
    {
        var authorDto = await _authorService.GetAllAuthorsAsync();

        return Ok(authorDto);
    }
    
    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> DeleteAuthorAsync(int authorId)
    {
        await _authorService.DeleteAuthorAsync(authorId);

        return Ok();
    }
}