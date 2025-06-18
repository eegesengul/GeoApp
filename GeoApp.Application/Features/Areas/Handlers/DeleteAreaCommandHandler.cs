using GeoApp.Application.Features.Areas.Commands;
using GeoApp.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteAreaCommandHandler : IRequestHandler<DeleteAreaCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteAreaCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (area == null)
            throw new Exception("Alan bulunamadı");

        if (!_currentUser.IsAdmin && area.CreatedByUserId != _currentUser.UserId)
            throw new UnauthorizedAccessException("Bu alanı silme yetkiniz yok.");

        _context.Areas.Remove(area);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
