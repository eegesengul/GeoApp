using Microsoft.AspNetCore.Identity;
using System;
using GeoApp.Infrastructure.Entities;
using GeoApp.Domain.Entities;

namespace GeoApp.Infrastructure.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
}
