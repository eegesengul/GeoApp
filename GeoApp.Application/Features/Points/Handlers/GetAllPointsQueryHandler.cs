using GeoApp.Application.Features.Points.Queries;
using GeoApp.Application.Interfaces;
using GeoApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoApp.Application.Features.Points.Handlers
{
    public class GetAllPointsQueryHandler : IRequestHandler<GetAllPointsQuery, IEnumerable<Point>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetAllPointsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<Point>> Handle(GetAllPointsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Point> query = _context.Points;

            if (!_currentUser.IsAdmin)
            {
                // Yalnızca oturum açmış kullanıcının noktalarını getir
                query = query.Where(p => p.CreatedByUserId == _currentUser.UserId);
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}
