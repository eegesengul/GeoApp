using GeoApp.Application.Features.Areas.Commands;
using GeoApp.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;

public class UpdateAreaCommandHandler : IRequestHandler<UpdateAreaCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateAreaCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (area == null)
            throw new Exception("Alan bulunamadı");

        if (!_currentUser.IsAdmin && area.CreatedByUserId != _currentUser.UserId)
            throw new UnauthorizedAccessException("Bu alanı güncelleme yetkiniz yok.");

        var geometry = new WKTReader().Read(request.WKTGeometry);

        area.Name = request.Name;
        area.Description = request.Description;
        area.Geometry = geometry;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
