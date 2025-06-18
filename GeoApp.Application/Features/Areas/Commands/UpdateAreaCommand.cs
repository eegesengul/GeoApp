using MediatR;

namespace GeoApp.Application.Features.Areas.Commands
{
    public class UpdateAreaCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string WKTGeometry { get; set; } = string.Empty;
    }
}
