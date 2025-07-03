using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace GeoApp.Application.Features.Auth.Commands
{
    public class LoginUserCommand : IRequest<AuthResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
