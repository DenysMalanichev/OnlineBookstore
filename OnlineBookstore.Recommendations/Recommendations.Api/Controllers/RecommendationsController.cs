using Microsoft.AspNetCore.Mvc;
using Recommendations.Abstractions.Services.Interfaces;

namespace Recommendations.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        [HttpGet]
        [ResponseCache(Duration = 1000, VaryByQueryKeys = new[] { "userId", "pageNumber", "pageSize" })]
        public async Task<IActionResult> GeneratePersonalRecommendationsAsync(
            [FromServices] IBookService bookService,
            [FromQuery] string userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var recommendation = await bookService.GenerateRecommendationsAsync(userId, pageNumber, pageSize);
            return Ok(recommendation);
        }
    }
}
