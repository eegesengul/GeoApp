using MediatR;

namespace GeoApp.Application.Features.Users.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
    }
}