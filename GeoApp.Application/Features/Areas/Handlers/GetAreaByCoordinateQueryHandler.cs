using GeoApp.Application.Features.Areas.Queries;
using GeoApp.Application.Interfaces;
using GeoApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GeoApp.Application.Features.Areas.Handlers
{
    public class GetAreaByCoordinateQueryHandler : IRequestHandler<GetAreaByCoordinateQuery, AreaDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetAreaByCoordinateQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<AreaDto?> Handle(GetAreaByCoordinateQuery request, CancellationToken cancellationToken)
        {
            var point = new Point(request.Longitude, request.Latitude) { SRID = 4326 };

            var query = _context.Areas
                .Where(area => area.Geometry.Contains(point));

            if (!_currentUser.IsAdmin)
                query = query.Where(area => area.CreatedByUserId == _currentUser.UserId);

            var area = await query.FirstOrDefaultAsync(cancellationToken);
            if (area == null) return null;

            return new AreaDto
            {
                Id = area.Id,
                Name = area.Name,
                Description = area.Description,
                Geometry = area.Geometry
            };
        }
    }
}
