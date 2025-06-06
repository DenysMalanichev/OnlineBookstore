using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Features.PublisherFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublishersController : ControllerBase
{
    private readonly IPublisherService _publisherService;

    public PublishersController(IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }

    [HttpGet("{publisherId:int}")]
    public async Task<IActionResult> GetPublisherByIdAsync(int publisherId)
    {
        var publisher = await _publisherService.GetPublisherByIdAsync(publisherId);

        return Ok(publisher);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllPublishersAsync()
    {
        var publishersDto = await _publisherService.GetAllPublishersAsync();

        return Ok(publishersDto);
    }
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> CreatePublisherAsync(CreatePublisherDto createPublisherDto)
    {
        await _publisherService.AddPublisherAsync(createPublisherDto);

        return Ok();
    }
    
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> UpdatePublisherAsync(UpdatePublisherDto updatePublisherDto)
    {
        await _publisherService.UpdatePublisherAsync(updatePublisherDto);

        return Ok();
    }
    
    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(RoleName.Admin))]
    public async Task<IActionResult> DeletePublisherAsync(int publisherId)
    {
        await _publisherService.DeletePublisherAsync(publisherId);

        return Ok();
    }
}