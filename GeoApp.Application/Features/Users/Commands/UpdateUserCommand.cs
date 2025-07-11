using MediatR;

namespace GeoApp.Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}