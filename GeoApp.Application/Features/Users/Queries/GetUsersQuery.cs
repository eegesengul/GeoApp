using MediatR;
using GeoApp.Application.Dtos;
using System.Collections.Generic;

namespace GeoApp.Application.Features.Users.Queries
{
    public class GetUsersQuery : IRequest<List<UserDto>>
    {
    }
}