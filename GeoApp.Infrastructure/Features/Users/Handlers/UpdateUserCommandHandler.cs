using MediatR;
using Microsoft.AspNetCore.Identity;
using GeoApp.Infrastructure.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeoApp.Application.Features.Users.Commands;

namespace GeoApp.Infrastructure.Features.Users.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly UserManager<AppUser> _userManager;
        public UpdateUserCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
                return false;

            user.UserName = request.Username;
            user.Email = request.Email;

            // Sadece request.Role boş değilse rol işlemleri yap
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                    await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            // Şifre güncelleme
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
                if (!passResult.Succeeded)
                    return false;
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}