using MediatR;

namespace GeoApp.Application.Features.Areas.Commands
{
    public class CreateAreaCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string GeoJsonGeometry { get; set; } = string.Empty;
    }
}