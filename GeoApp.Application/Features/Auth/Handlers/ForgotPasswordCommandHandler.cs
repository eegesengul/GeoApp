using MediatR;
using GeoApp.Application.Features.Auth.Commands;
using GeoApp.Application.Interfaces; // DEĞİŞTİ
using System.Threading;
using System.Threading.Tasks;

namespace GeoApp.Application.Features.Auth.Handlers
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, AuthResult>
    {
        private readonly IIdentityService _identityService; // DEĞİŞTİ

        public ForgotPasswordCommandHandler(IIdentityService identityService) // DEĞİŞTİ
        {
            _identityService = identityService;
        }

        public async Task<AuthResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // Sadece ilgili servisi çağır
            return await _identityService.ForgotPasswordAsync(request.Email, request.Origin);
        }
    }
}