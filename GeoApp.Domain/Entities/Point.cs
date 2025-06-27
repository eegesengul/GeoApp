using NetTopologySuite.Geometries;
using System;

namespace GeoApp.Domain.Entities
{
    public class Point
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Geometry Geometry { get; set; } = default!;
        public Guid CreatedByUserId { get; set; }
    }
}