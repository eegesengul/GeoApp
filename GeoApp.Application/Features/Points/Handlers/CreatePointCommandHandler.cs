using GeoApp.Application.Features.Points.Commands;
using GeoApp.Application.Interfaces;
// using GeoApp.Domain.Entities; // Bu satırı doğrudan kullanmak yerine tam adresi kullanacağız.
using MediatR;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GeoApp.Application.Features.Points.Handlers
{
    public class CreatePointCommandHandler : IRequestHandler<CreatePointCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CreatePointCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(CreatePointCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.GeoJsonGeometry))
            {
                throw new ArgumentException("GeoJSON geometry cannot be null or empty.", nameof(request.GeoJsonGeometry));
            }

            var reader = new GeoJsonReader();
            Geometry geometry;

            try
            {
                using (var jsonDoc = JsonDocument.Parse(request.GeoJsonGeometry))
                {
                    if (jsonDoc.RootElement.TryGetProperty("geometry", out var geometryElement))
                    {
                        var geometryNode = geometryElement.ToString();
                        geometry = reader.Read<Geometry>(geometryNode);
                    }
                    else
                    {
                        geometry = reader.Read<Geometry>(request.GeoJsonGeometry);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Invalid GeoJSON geometry. Details: {ex.Message}", ex);
            }

            if (geometry.OgcGeometryType != OgcGeometryType.Point)
            {
                throw new ArgumentException("The provided geometry is not a point.", nameof(request.GeoJsonGeometry));
            }

            // Hatanın çözümü için burada tam adres (fully qualified name) kullanılıyor.
            var point = new Domain.Entities.Point
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Geometry = geometry,
                CreatedByUserId = _currentUser.UserId
            };

            _context.Points.Add(point);
            await _context.SaveChangesAsync(cancellationToken);

            return point.Id;
        }
    }
}