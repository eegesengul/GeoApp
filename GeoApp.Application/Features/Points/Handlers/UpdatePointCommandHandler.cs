using GeoApp.Application.Features.Points.Commands;
using GeoApp.Application.Interfaces;
using GeoApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeoApp.Application.Features.Points.Handlers
{
    // DÜZELTME: IRequestHandler<UpdatePointCommand, Unit> olarak değiştirildi.
    public class UpdatePointCommandHandler : IRequestHandler<UpdatePointCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser; // DÜZELTME: Yetki kontrolü için eklendi.

        public UpdatePointCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser; // DÜZELTME: Yetki kontrolü için eklendi.
        }

        // DÜZELTME: Dönüş tipi Task<Unit> olarak değiştirildi.
        public async Task<Unit> Handle(UpdatePointCommand request, CancellationToken cancellationToken)
        {
            var pointToUpdate = await _context.Points.FindAsync(new object[] { request.Id }, cancellationToken);

            if (pointToUpdate == null)
            {
                // DÜZELTME: Proje desenine uygun hata fırlatma.
                throw new Exception("Nokta bulunamadı");
            }

            // DÜZELTME: Yetki kontrolü eklendi.
            if (!_currentUser.IsAdmin && pointToUpdate.CreatedByUserId != _currentUser.UserId)
            {
                throw new UnauthorizedAccessException("Bu noktayı güncelleme yetkiniz yok.");
            }

            pointToUpdate.Name = request.Name;
            pointToUpdate.Description = request.Description;

            if (!string.IsNullOrWhiteSpace(request.GeoJsonGeometry))
            {
                var reader = new GeoJsonReader();
                Geometry geometry;
                try
                {
                    geometry = reader.Read<Geometry>(request.GeoJsonGeometry);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Invalid GeoJSON geometry. Details: {ex.Message}", ex);
                }

                if (geometry.OgcGeometryType != OgcGeometryType.Point)
                {
                    throw new ArgumentException("The provided geometry must be a Point.", nameof(request.GeoJsonGeometry));
                }
                pointToUpdate.Geometry = geometry;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // DÜZELTME: Unit.Value döndürülüyor.
            return Unit.Value;
        }
    }
}