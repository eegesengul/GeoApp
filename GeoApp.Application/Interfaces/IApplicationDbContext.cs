using Microsoft.EntityFrameworkCore;
using GeoApp.Domain.Entities;
using System.Collections.Generic;

namespace GeoApp.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Area> Areas { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}