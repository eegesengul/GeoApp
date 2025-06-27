using MediatR;
using System;

namespace GeoApp.Application.Features.Points.Commands
{
    public class CreatePointCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string GeoJsonGeometry { get; set; } = string.Empty;
    }
}