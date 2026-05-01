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

        /// <summary>
        /// Endpoint que obtienes las puntuaciones recientes para un usuario en concreto
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="limit">Cantidad de puntuaciones</param>
        /// <returns></returns>
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentScores([FromQuery] int userId, [FromQuery] int limit = 4)
        {
            var (success, message, scores) = await _scoreService.GetRecentScoresForUser(userId, limit);

            if (success)
                return Ok(new { message, scores });

            return StatusCode(StatusCodes.Status500InternalServerError, new { message });
        }

        /// <summary>
        /// Endpoint que obtiene la última puntuación conseguida por un usuario
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <returns></returns>
        [HttpGet("last")]
        public async Task<IActionResult> GetLastScore([FromQuery] int userId)
        {
            var (success, message, score) = await _scoreService.GetLastScoreForUser(userId);

            if (success)
                return Ok(new { message, score });

            return StatusCode(StatusCodes.Status500InternalServerError, new { message });
        }
    }
}