using Microsoft.EntityFrameworkCore;
using GeoApp.Domain.Entities;

namespace GeoApp.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Area> Areas { get; }
        DbSet<Point> Points { get; }
       
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}