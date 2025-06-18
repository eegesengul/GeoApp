using GeoApp.Application.Dtos;
using GeoApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;


namespace GeoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var token = await _authService.RegisterAsync(dto);
            if (token == null)
                return BadRequest("Kayıt başarısız.");

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
                return Unauthorized("Giriş başarısız.");

            return Ok(new { token });
        }
    }
}
