using GeoApp.Domain.Entities;
using MediatR;
using System;

namespace GeoApp.Application.Features.Points.Queries
{
    public class GetPointByIdQuery : IRequest<Point>
    {
        public Guid Id { get; }

        public GetPointByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}