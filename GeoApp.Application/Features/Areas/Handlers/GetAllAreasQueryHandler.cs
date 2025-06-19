using AutoMapper;
using GeoApp.Application.Dtos;
using GeoApp.Application.Features.Areas.Queries;
using GeoApp.Application.Interfaces;
using GeoApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeoApp.Application.Features.Areas.Handlers
{
    public class GetAllAreasQueryHandler : IRequestHandler<GetAllAreasQuery, IEnumerable<AreaDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public GetAllAreasQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
        {
            _context = context;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AreaDto>> Handle(GetAllAreasQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Area> query = _context.Areas;

            if (!_currentUser.IsAdmin)
            {
                // Sadece kendi oluşturduğu alanları getir
                query = query.Where(area => area.CreatedByUserId == _currentUser.UserId);
            }

            var areas = await query.ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<AreaDto>>(areas);
        }
    }
}
