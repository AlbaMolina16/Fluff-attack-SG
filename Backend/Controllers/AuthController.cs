using FluffGameApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FluffGameApi.Controllers
{
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
        //public async Task<IActionResult> Login(LoginDto dto)
        public async Task<IActionResult> Login(string username)
        {
            var result = await _authService.Login(username);
            return Ok(result);
        }

    }
}
