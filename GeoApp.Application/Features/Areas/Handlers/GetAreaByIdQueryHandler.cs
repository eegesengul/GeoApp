using GeoApp.Application.Features.Areas.Queries;
using GeoApp.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeoApp.Application.Features.Areas.Handlers
{
    public class GetAreaByIdQueryHandler : IRequestHandler<GetAreaByIdQuery, AreaDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetAreaByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<AreaDto> Handle(GetAreaByIdQuery request, CancellationToken cancellationToken)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (area == null)
                throw new Exception("Alan bulunamadı.");

            if (!_currentUser.IsAdmin && area.CreatedByUserId != _currentUser.UserId)
                throw new UnauthorizedAccessException("Bu alana erişim izniniz yok.");

            return new AreaDto
            {
                Id = area.Id,
                Name = area.Name,
                Description = area.Description,
                Geometry = area.Geometry
            };
        }
    }
}
