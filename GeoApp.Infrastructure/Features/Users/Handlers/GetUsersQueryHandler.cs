using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GeoApp.Application.Common.Models;
using GeoApp.Application.Dtos;
using GeoApp.Application.Features.Users.Queries;
using GeoApp.Infrastructure.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoApp.Infrastructure.Features.Users.Handlers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        public GetUsersQueryHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            // Sayfalamanın 1'den başladığından emin olalım
            if (request.Page < 1) request.Page = 1;
            if (request.PageSize < 1) request.PageSize = 20;
            if (request.PageSize > 100) request.PageSize = 100; // Maksimum sayfa boyutunu sınırlayalım

            // Toplam kullanıcı sayısını al
            var totalCount = await _userManager.Users.CountAsync(cancellationToken);

            // Sayfalama ile kullanıcıları al
            var users = await _userManager.Users
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? ""
                });
            }

            return new PagedResult<UserDto>
            {
                Items = result,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}