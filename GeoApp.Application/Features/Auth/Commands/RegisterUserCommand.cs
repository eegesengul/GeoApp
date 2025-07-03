using MediatR;

namespace GeoApp.Application.Features.Auth.Commands
{
    public class RegisterUserCommand : IRequest<AuthResult>
    {
        // RegisterDto'dan gelen alanlar
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}