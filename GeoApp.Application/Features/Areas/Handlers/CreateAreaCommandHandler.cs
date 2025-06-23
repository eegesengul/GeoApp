using GeoApp.Application.Features.Areas.Commands;
using GeoApp.Application.Interfaces;
using GeoApp.Domain.Entities;
using MediatR;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Text.Json;

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
            var reader = new GeoJsonReader();

            try
            {
                using (var jsonDoc = JsonDocument.Parse(request.GeoJsonGeometry))
                {
                    var geometryNode = jsonDoc.RootElement.GetProperty("geometry").ToString();
                    geometry = reader.Read<Geometry>(geometryNode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Geçersiz GeoJSON geometrisi. Hata: {ex.Message}");
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