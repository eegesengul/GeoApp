using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace GeoApp.Application.Features.Auth.Commands
{
    public class ResetPasswordCommand : IRequest<AuthResult>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}