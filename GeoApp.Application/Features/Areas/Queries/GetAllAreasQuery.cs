using GeoApp.Application.Dtos;
using MediatR;

namespace GeoApp.Application.Features.Areas.Queries
{
    public class GetAllAreasQuery : IRequest<IEnumerable<AreaDto>> { }
}
