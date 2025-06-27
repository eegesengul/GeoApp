using Microsoft.EntityFrameworkCore;
using GeoApp.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace GeoApp.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Area> Areas { get; }
        DbSet<Point> Points { get; } // Point için DbSet eklendi

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}