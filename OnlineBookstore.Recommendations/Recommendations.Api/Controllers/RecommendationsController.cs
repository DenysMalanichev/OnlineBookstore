using Microsoft.AspNetCore.Mvc;
using Recommendations.Abstractions.Services.Interfaces;

namespace Recommendations.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GeneratePersonalRecommendationsAsync(
            string userId, [FromServices] IBookService bookService)
        {
            return Ok(await bookService.GenerateRecommendationsAsync(userId));
        }
    }
}
