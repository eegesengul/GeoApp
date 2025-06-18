using MediatR;
using NetTopologySuite.Geometries;

namespace GeoApp.Application.Features.Areas.Queries
{
    public class GetAllAreasQuery : IRequest<IEnumerable<AreaDto>> { }

    public class AreaDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Geometry Geometry { get; set; } = default!;
    }
}
