using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Application.Services.Interfaces;
using OnlineBookstore.Features.PublisherFeatures;

namespace OnlineBookstore.Controllers;

[ApiController]
[Route("[controller]")]
public class PublishersController : ControllerBase
{
    private readonly IPublisherService _publisherService;

    public PublishersController(IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPublisherByIdAsync(int publisherId)
    {
        var publisher = await _publisherService.GetPublisherByIdAsync(publisherId);

        return Ok(publisher);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePublisherAsync(CreatePublisherDto createPublisherDto)
    {
        await _publisherService.AddPublisherAsync(createPublisherDto);

        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdatePublisherAsync(UpdatePublisherDto updatePublisherDto)
    {
        await _publisherService.UpdatePublisherAsync(updatePublisherDto);

        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeletePublisherAsync(int publisherId)
    {
        await _publisherService.DeletePublisherAsync(publisherId);

        return Ok();
    }
}