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
            // Gelen GeoJSON verisinin boş olup olmadığını kontrol et
            if (string.IsNullOrWhiteSpace(request.GeoJsonGeometry))
            {
                throw new ArgumentException("GeoJSON geometry cannot be null or empty.", nameof(request.GeoJsonGeometry));
            }

            Geometry geometry;
            var reader = new GeoJsonReader();

            try
            {
                // Gelen veri bir GeoJSON Feature ise, içinden 'geometry' kısmını al
                using (var jsonDoc = JsonDocument.Parse(request.GeoJsonGeometry))
                {
                    if (jsonDoc.RootElement.TryGetProperty("geometry", out var geometryElement))
                    {
                        var geometryNode = geometryElement.ToString();
                        geometry = reader.Read<Geometry>(geometryNode);
                    }
                    else
                    {
                        // Eğer 'geometry' propertysi yoksa, gelen verinin kendisinin bir geometri olduğunu varsay
                        geometry = reader.Read<Geometry>(request.GeoJsonGeometry);
                    }
                }
            }
            catch (Exception ex)
            {
                // Orijinal hatayı kaybetmeden yeni bir hata fırlat
                throw new Exception($"Invalid GeoJSON geometry. Details: {ex.Message}", ex);
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