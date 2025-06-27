using GeoApp.Domain.Entities;
using GeoApp.Infrastructure.Entities;
using GeoApp.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeoApp.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Area> Areas => Set<Area>();
        public DbSet<Point> Points => Set<Point>(); // Point için DbSet eklendi

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Area>()
                .Property(a => a.Geometry)
                .HasColumnType("geometry")
                .HasAnnotation("Relational:SRID", 4326);

            // Point için geometri konfigürasyonu eklendi
            modelBuilder.Entity<Point>()
                .Property(p => p.Geometry)
                .HasColumnType("geometry")
                .HasAnnotation("Relational:SRID", 4326);
        }
    }
}