using FluffGameApi.Dtos;
using FluffGameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FluffGameApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var (success, message, users) = await _authService.GetAllUsers();
            if (success)
            {
                return Ok(new { message, users });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.Login(loginDto);

            if (!result.success)
            {
                return Unauthorized(new { result.message });
            }

            return Ok(new { result.success, result.message, result.idUsuario });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.Register(registerDto);

            if (!result.success)
                return Conflict(new { result.message });

            return Ok(result);
        }

    }
}