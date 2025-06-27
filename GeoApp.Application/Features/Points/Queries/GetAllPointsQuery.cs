using GeoApp.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace GeoApp.Application.Features.Points.Queries
{
    public class GetAllPointsQuery : IRequest<IEnumerable<Point>>
    {
    }
}