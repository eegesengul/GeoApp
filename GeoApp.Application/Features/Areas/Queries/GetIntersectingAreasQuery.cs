using GeoApp.Application.Dtos;
using MediatR;

namespace GeoApp.Application.Features.Areas.Queries
{
    public class GetIntersectingAreasQuery : IRequest<IEnumerable<AreaDto>>
    {
        public string WKTGeometry { get; set; } = string.Empty;

        public GetIntersectingAreasQuery(string wktGeometry)
        {
            WKTGeometry = wktGeometry;
        }
    }
}
