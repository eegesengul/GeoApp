using MediatR;
using GeoApp.Application.Dtos;

namespace GeoApp.Application.Features.Users.Queries
{
    public class GetMyUserQuery : IRequest<UserDto>
    {
        public string UserId { get; set; } = default!;
    }
}