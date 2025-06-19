using GeoApp.Application.Features.Areas.Queries;
using GeoApp.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeoApp.Application.Features.Areas.Handlers
{
    public class GetAreaSizeQueryHandler : IRequestHandler<GetAreaSizeQuery, double>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetAreaSizeQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<double> Handle(GetAreaSizeQuery request, CancellationToken cancellationToken)
        {
            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == request.AreaId, cancellationToken);

            if (area == null)
                throw new Exception("Alan bulunamadý");

            if (!_currentUser.IsAdmin && area.CreatedByUserId != _currentUser.UserId)
                throw new UnauthorizedAccessException("Bu alana eriþim izniniz yok");

            // ST_Area kullanarak alan hesaplama (metrekare cinsinden)
            return area.Geometry.Area;
        }
    }
}