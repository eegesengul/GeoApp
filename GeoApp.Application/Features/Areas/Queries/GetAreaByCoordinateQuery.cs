using GeoApp.Application.Dtos;
using MediatR;

namespace GeoApp.Application.Features.Areas.Queries
{
    public class GetAreaByCoordinateQuery : IRequest<AreaDto?>
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GetAreaByCoordinateQuery(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
