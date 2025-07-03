using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace GeoApp.Application.Features.Auth.Commands
{
    public class ForgotPasswordCommand : IRequest<AuthResult>
    {
        public string Email { get; set; }
        // Frontend'den gelen reset linkinin base URL'ini buraya taşıyoruz ki Handler'da kullanabilelim.
        public string Origin { get; set; }
    }
}
