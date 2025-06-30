using Microsoft.AspNetCore.Identity;
using System;

namespace GeoApp.Infrastructure.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
}