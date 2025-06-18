using GeoApp.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GeoApp.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public Guid UserId { get; }
        public string Role { get; } = string.Empty;
        public bool IsAdmin => Role == "ADMIN";

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

                if (Guid.TryParse(nameIdentifier, out var userId))
                    UserId = userId;

                if (!string.IsNullOrEmpty(roleClaim))
                    Role = roleClaim;
            }
        }
    }
}
