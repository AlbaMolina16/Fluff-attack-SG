using FluffGameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FluffGameApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreController : ControllerBase
    {
        private readonly ScoreService _scoreService;

        public ScoreController(ScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentScores([FromQuery] int userId, [FromQuery] int limit = 4)
        {
            var (success, message, scores) = await _scoreService.GetRecentScoresForUser(userId, limit);

            if (success)
                return Ok(new { message, scores });

            return StatusCode(StatusCodes.Status500InternalServerError, new { message });
        }
    }
}