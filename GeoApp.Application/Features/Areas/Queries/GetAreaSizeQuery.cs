using MediatR;

namespace GeoApp.Application.Features.Areas.Queries
{
    public class GetAreaSizeQuery : IRequest<double>
    {
        public Guid AreaId { get; set; }

        public GetAreaSizeQuery(Guid areaId)
        {
            AreaId = areaId;
        }
    }
}