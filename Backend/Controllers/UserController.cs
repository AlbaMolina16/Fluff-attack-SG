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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto newUserDto)
        {
            var result = await _authService.Register(newUserDto);

            if (!result.success)
                return Conflict(new { result.message });

            return Ok(result);
        }

        [HttpPut("preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesDto dto)
        {
            var (success, message) = await _authService.UpdatePreferences(dto);

            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message });

            return Ok(new { message });
        }
    }
}