using GeoApp.Application.Dtos;
using NetTopologySuite.Geometries;

namespace GeoApp.API.Helpers
{
    public static class GeoJsonHelper
    {
        public static object ToGeoJson(IEnumerable<AreaDto> areas)
        {
            var features = areas.Select(area => new
            {
                type = "Feature",
                geometry = new
                {
                    type = "Polygon",
                    coordinates = new[]
                    {
                        area.Geometry.Coordinates.Select(c => new[] { c.X, c.Y }).ToArray()
                    }
                },
                properties = new
                {
                    area.Id,
                    area.Name,
                    area.Description
                }
            });

            return new
            {
                type = "FeatureCollection",
                features
            };
        }

        public static object ToGeoJson(AreaDto area)
        {
            return new
            {
                type = "Feature",
                geometry = new
                {
                    type = "Polygon",
                    coordinates = new[]
                    {
                        area.Geometry.Coordinates.Select(c => new[] { c.X, c.Y }).ToArray()
                    }
                },
                properties = new
                {
                    area.Id,
                    area.Name,
                    area.Description
                }
            };
        }
    }
}