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

        /// <summary>
        /// Endpoint de login
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Endpoint de creación de usuario
        /// </summary>
        /// <param name="newUserDto">Información del usuario que se quiere crear</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto newUserDto)
        {
            var result = await _authService.Register(newUserDto);

            if (!result.success)
                return Conflict(new { result.message });

            return Ok(result);
        }

    }
}