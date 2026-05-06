using FluffGameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FluffGameApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DifficultyController : ControllerBase
    {
        private readonly DifficultyService _difficultyService;

        public DifficultyController(DifficultyService difficultyService)
        {
            _difficultyService = difficultyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var (success, message, difficulties) = await _difficultyService.GetAll();

            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message });

            return Ok(new { message, difficulties });
        }
    }
}
