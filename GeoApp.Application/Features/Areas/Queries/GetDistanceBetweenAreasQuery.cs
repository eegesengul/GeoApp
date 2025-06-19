using MediatR;

namespace GeoApp.Application.Features.Areas.Queries
{
    public class GetDistanceBetweenAreasQuery : IRequest<double>
    {
        public Guid FirstAreaId { get; set; }
        public Guid SecondAreaId { get; set; }

        public GetDistanceBetweenAreasQuery(Guid firstAreaId, Guid secondAreaId)
        {
            FirstAreaId = firstAreaId;
            SecondAreaId = secondAreaId;
        }
    }
}