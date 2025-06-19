using GeoApp.Application.Features.Areas.Queries;
using GeoApp.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeoApp.Application.Features.Areas.Handlers
{
    public class GetDistanceBetweenAreasQueryHandler : IRequestHandler<GetDistanceBetweenAreasQuery, double>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetDistanceBetweenAreasQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<double> Handle(GetDistanceBetweenAreasQuery request, CancellationToken cancellationToken)
        {
            var firstArea = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == request.FirstAreaId, cancellationToken);
            
            var secondArea = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == request.SecondAreaId, cancellationToken);

            if (firstArea == null || secondArea == null)
                throw new Exception("Alanlardan biri veya her ikisi bulunamadý");

            // Kullanýcý her iki alana da eriþebilmeli
            if (!_currentUser.IsAdmin)
            {
                bool canAccessFirst = firstArea.CreatedByUserId == _currentUser.UserId;
                bool canAccessSecond = secondArea.CreatedByUserId == _currentUser.UserId;

                if (!canAccessFirst || !canAccessSecond)
                    throw new UnauthorizedAccessException("Bu alanlardan birine veya her ikisine eriþim izniniz yok");
            }

            // Ýki geometri arasýndaki mesafeyi hesapla (metre cinsinden)
            return firstArea.Geometry.Distance(secondArea.Geometry);
        }
    }
}