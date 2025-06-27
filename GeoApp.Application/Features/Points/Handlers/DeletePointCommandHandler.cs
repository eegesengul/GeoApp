using GeoApp.Application.Features.Points.Commands;
using GeoApp.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore; // DÜZELTME: FirstOrDefaultAsync için eklendi.
using System; // DÜZELTME: UnauthorizedAccessException için eklendi.
using System.Threading;
using System.Threading.Tasks;

namespace GeoApp.Application.Features.Points.Handlers
{
    // DÜZELTME: IRequestHandler<DeletePointCommand, Unit> olarak değiştirildi.
    public class DeletePointCommandHandler : IRequestHandler<DeletePointCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser; // DÜZELTME: Yetki kontrolü için eklendi.

        public DeletePointCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser; // DÜZELTME: Yetki kontrolü için eklendi.
        }

        // DÜZELTME: Dönüş tipi Task<Unit> olarak değiştirildi.
        public async Task<Unit> Handle(DeletePointCommand request, CancellationToken cancellationToken)
        {
            // DÜZELTME: FindAsync yerine FirstOrDefaultAsync kullanıldı.
            var pointToDelete = await _context.Points
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (pointToDelete == null)
            {
                // DÜZELTME: Proje desenine uygun hata fırlatma.
                throw new Exception("Nokta bulunamadı");
            }

            // DÜZELTME: Yetki kontrolü eklendi.
            if (!_currentUser.IsAdmin && pointToDelete.CreatedByUserId != _currentUser.UserId)
            {
                throw new UnauthorizedAccessException("Bu noktayı silme yetkiniz yok.");
            }

            _context.Points.Remove(pointToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            // DÜZELTME: Unit.Value döndürülüyor.
            return Unit.Value;
        }
    }
}