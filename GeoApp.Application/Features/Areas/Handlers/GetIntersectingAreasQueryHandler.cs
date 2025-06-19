using GeoApp.Application.Dtos;
using GeoApp.Application.Features.Areas.Queries;
using GeoApp.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using AutoMapper;

namespace GeoApp.Application.Features.Areas.Handlers
{
    public class GetIntersectingAreasQueryHandler : IRequestHandler<GetIntersectingAreasQuery, IEnumerable<AreaDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public GetIntersectingAreasQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
        {
            _context = context;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AreaDto>> Handle(GetIntersectingAreasQuery request, CancellationToken cancellationToken)
        {
            var reader = new WKTReader();
            var geometry = reader.Read(request.WKTGeometry);
            geometry.SRID = 4326;

            var query = _context.Areas
                .Where(a => a.Geometry.Intersects(geometry));

            if (!_currentUser.IsAdmin)
                query = query.Where(a => a.CreatedByUserId == _currentUser.UserId);

            var intersecting = await query.ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<AreaDto>>(intersecting);
        }
    }
}
