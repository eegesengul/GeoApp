using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoApp.Application.Features.Auth
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
        public bool IsLockedOut { get; set; }

        // Token'lı başarılı sonuç
        public static AuthResult Success(string token) => new() { Succeeded = true, Token = token };

        // YENİ - Token'sız başarılı sonuç
        public static AuthResult Success() => new() { Succeeded = true };

        public static AuthResult Failure(IEnumerable<string> errors) => new() { Errors = errors };
        public static AuthResult LockedOut() => new() { IsLockedOut = true };
    }
}