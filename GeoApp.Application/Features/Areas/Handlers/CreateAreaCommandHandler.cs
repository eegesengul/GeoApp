using GeoApp.Application.Features.Areas.Commands;
using GeoApp.Application.Interfaces;
using GeoApp.Domain.Entities;
using MediatR;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace GeoApp.Application.Features.Areas.Handlers
{
    public class CreateAreaCommandHandler : IRequestHandler<CreateAreaCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CreateAreaCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
        {
            Geometry geometry;

            try
            {
                geometry = new WKTReader().Read(request.WKTGeometry);
            }
            catch
            {
                throw new Exception("Geçersiz WKT geometrisi.");
            }

            var area = new Area
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Geometry = geometry,
                CreatedByUserId = _currentUser.UserId
            };

            _context.Areas.Add(area);
            await _context.SaveChangesAsync(cancellationToken);

            return area.Id;
        }
    }
}
