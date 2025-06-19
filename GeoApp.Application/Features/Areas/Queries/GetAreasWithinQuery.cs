using GeoApp.Application.Dtos;
using MediatR;

namespace GeoApp.Application.Features.Areas.Queries
{
    public class GetAreasWithinQuery : IRequest<IEnumerable<AreaDto>>
    {
        public string WKTGeometry { get; set; } = string.Empty;

        public GetAreasWithinQuery(string wktGeometry)
        {
            WKTGeometry = wktGeometry;
        }
    }
}