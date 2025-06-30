using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using GeoApp.Application.Features.Users.Queries;
using GeoApp.Application.Dtos;
using GeoApp.Infrastructure.Persistence; // ApplicationDbContext burada

namespace GeoApp.Infrastructure.Features.Users.Handlers
{
    public class GetMyUserQueryHandler : IRequestHandler<GetMyUserQuery, UserDto?>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetMyUserQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle(GetMyUserQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.UserId, out Guid userGuid))
                return null;

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userGuid, cancellationToken);

            return user == null ? null : _mapper.Map<UserDto>(user);
        }
    }
}