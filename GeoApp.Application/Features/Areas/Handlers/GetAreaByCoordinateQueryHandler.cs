using GeoApp.Application.Dtos;
using GeoApp.Application.Features.Areas.Queries;
using GeoApp.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using AutoMapper;

namespace GeoApp.Application.Features.Areas.Handlers
{
    public class GetAreaByCoordinateQueryHandler : IRequestHandler<GetAreaByCoordinateQuery, AreaDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public GetAreaByCoordinateQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
        {
            _context = context;
            _currentUser = currentUser;
            _mapper = mapper;
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

            return _mapper.Map<AreaDto>(area);
        }
    }
}
