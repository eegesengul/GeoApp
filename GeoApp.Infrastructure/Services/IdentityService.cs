using GeoApp.Application.Dtos;
using GeoApp.Application.Features.Auth;
using GeoApp.Application.Interfaces;
using GeoApp.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Threading.Tasks;

namespace GeoApp.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;

        public IdentityService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResult> RegisterUserAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return AuthResult.Failure(new[] { "Bu e-posta adresi zaten kullanılıyor." });

            var newUser = new AppUser { UserName = dto.UserName, Email = dto.Email, FullName = dto.FullName };
            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
                return AuthResult.Failure(result.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(newUser, "USER");
            var roles = await _userManager.GetRolesAsync(newUser);
            var token = _tokenService.CreateToken(newUser, roles);
            return AuthResult.Success(token);
        }

        public async Task<AuthResult> LoginUserAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return AuthResult.Failure(new[] { "Geçersiz e-posta veya şifre." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, true);
            if (result.IsLockedOut)
                return AuthResult.LockedOut();
            if (!result.Succeeded)
                return AuthResult.Failure(new[] { "Geçersiz e-posta veya şifre." });

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);
            return AuthResult.Success(token);
        }

        public async Task<AuthResult> ForgotPasswordAsync(string email, string origin)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return AuthResult.Success(); // Güvenlik için her zaman başarılı dön

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var resetLink = $"{origin}/reset-password?token={encodedToken}&email={WebUtility.UrlEncode(user.Email)}";

            // TODO: E-posta gönderme servisini burada çağır.
            Console.WriteLine($"Password Reset Link: {resetLink}");

            return AuthResult.Success();
        }

        public async Task<AuthResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return AuthResult.Failure(new[] { "İşlem başarısız." });

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                return AuthResult.Failure(result.Errors.Select(e => e.Description));

            return AuthResult.Success();
        }
    }
}