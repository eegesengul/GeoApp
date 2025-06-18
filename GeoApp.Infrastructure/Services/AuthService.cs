using GeoApp.Application.Dtos;
using GeoApp.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace GeoApp.Infrastructure.Services
{
    public class AuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;

        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<string?> RegisterAsync(RegisterDto dto)
        {
            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                Role = "User"
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded) return null;

            await _userManager.AddToRoleAsync(user, "User");

            return _tokenService.CreateToken(user, new List<string> { "User" });
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return _tokenService.CreateToken(user, roles);
        }
    }
}
