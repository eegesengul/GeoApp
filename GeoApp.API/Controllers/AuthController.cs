using GeoApp.Application.Dtos;
using GeoApp.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var command = new RegisterUserCommand
            {
                FullName = dto.FullName,
                Username = dto.UserName,
                Email = dto.Email,
                Password = dto.Password
            };
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(new { message = "Kayıt başarısız.", errors = result.Errors });
            return Ok(new { token = result.Token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var command = new LoginUserCommand { Email = dto.Email, Password = dto.Password };
            var result = await _mediator.Send(command);
            if (result.IsLockedOut)
                return Unauthorized(new { message = "Hesabınız çok sayıda başarısız deneme nedeniyle geçici olarak kilitlendi." });
            if (!result.Succeeded)
                return Unauthorized(new { message = "Geçersiz e-posta veya şifre." });
            return Ok(new { token = result.Token });
        }

        // YENİ - Şifre sıfırlama talebi endpoint'i
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var command = new ForgotPasswordCommand
            {
                Email = dto.Email,
                // Request'in geldiği origin'i (örn: http://localhost:4200) alıp command'e ekliyoruz.
                Origin = Request.Headers["origin"]
            };
            await _mediator.Send(command);
            // Her durumda aynı mesajı dönerek e-posta adreslerinin var olup olmadığını gizliyoruz.
            return Ok(new { message = "Talep başarılıysa, şifre sıfırlama talimatları e-posta adresinize gönderilecektir." });
        }

        // YENİ - Yeni şifreyi ayarlama endpoint'i
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var command = new ResetPasswordCommand
            {
                Email = dto.Email,
                Token = dto.Token,
                NewPassword = dto.NewPassword
            };
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(new { message = "Şifre sıfırlama başarısız oldu.", errors = result.Errors });
            return Ok(new { message = "Şifreniz başarıyla sıfırlandı." });
        }
    }
}