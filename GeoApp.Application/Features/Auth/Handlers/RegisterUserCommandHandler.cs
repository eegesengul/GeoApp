using MediatR;
using GeoApp.Application.Features.Auth.Commands;
using GeoApp.Application.Interfaces; // DEĞİŞTİ
using GeoApp.Application.Dtos;      // YENİ

namespace GeoApp.Application.Features.Auth.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResult>
    {
        private readonly IIdentityService _identityService; // DEĞİŞTİ

        public RegisterUserCommandHandler(IIdentityService identityService) // DEĞİŞTİ
        {
            _identityService = identityService;
        }

        public async Task<AuthResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Command'i, arayüzün beklediği DTO'ya dönüştür
            var registerDto = new RegisterDto
            {
                FullName = request.FullName,
                UserName = request.Username,
                Email = request.Email,
                Password = request.Password
            };

            // Tüm iş mantığını içeren servisi çağır
            return await _identityService.RegisterUserAsync(registerDto);
        }
    }
}