using MediatR;
using System;

namespace GeoApp.Application.Features.Points.Commands
{
    // DÜZELTME: IRequest<Unit> olarak değiştirildi.
    public class UpdatePointCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string GeoJsonGeometry { get; set; } = string.Empty;
    }
}