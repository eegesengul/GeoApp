using MediatR;
using GeoApp.Application.Dtos;
using GeoApp.Application.Common.Models;

namespace GeoApp.Application.Features.Users.Queries
{
    public class GetUsersQuery : IRequest<PagedResult<UserDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}