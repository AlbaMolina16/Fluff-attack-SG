using FluffGameApi.Dtos;
using FluffGameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FluffGameApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AuthService _authService;

        public UserController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var (success, message, users) = await _authService.GetAll();
            if (success)
                return Ok(new { message, users });

            return StatusCode(StatusCodes.Status500InternalServerError, new { message });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto newUserDto)
        {
            var result = await _authService.Register(newUserDto);

            if (!result.success)
                return Conflict(new { result.message });

            return Ok(result);
        }

        [HttpPut("preferences/{userId}")]
        public async Task<IActionResult> UpdatePreferences(int userId, [FromBody] UpdatePreferencesDto dto)
        {
            var (success, message) = await _authService.UpdatePreferences(userId, dto.IdDifficulty);

            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message });

            return Ok(new { message });
        }
    }
}