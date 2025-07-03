using MediatR;
using GeoApp.Application.Features.Auth.Commands;
using GeoApp.Application.Interfaces; // DEĞİŞTİ
using GeoApp.Application.Dtos;      // YENİ

namespace GeoApp.Application.Features.Auth.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResult>
    {
        private readonly IIdentityService _identityService; // DEĞİŞTİ

        public LoginUserCommandHandler(IIdentityService identityService) // DEĞİŞTİ
        {
            _identityService = identityService;
        }

        public async Task<AuthResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // Command'i, arayüzün beklediği DTO'ya dönüştür
            var loginDto = new LoginDto
            {
                Email = request.Email,
                Password = request.Password
            };

            // Tüm iş mantığını içeren servisi çağır
            return await _identityService.LoginUserAsync(loginDto);
        }
    }
}