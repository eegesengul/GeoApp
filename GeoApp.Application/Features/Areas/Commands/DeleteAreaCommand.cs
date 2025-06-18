using MediatR;

namespace GeoApp.Application.Features.Areas.Commands
{
    public class DeleteAreaCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
