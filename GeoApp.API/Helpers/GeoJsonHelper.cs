using GeoApp.Application.Dtos;
using System.Collections.Generic;
using System.Linq;

// DÝKKAT: NetTopologySuite using'leri artýk doðrudan burada kullanýlmýyor,
// çünkü her þeyi standart nesnelere çeviriyoruz.

namespace GeoApp.API.Helpers
{
    public static class GeoJsonHelper
    {
        // --- Area Metotlarý (Deðiþiklik yok) ---
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

        // --- Point Metotlarý (TAMAMEN YENÝDEN YAZILDI) ---

        // DÜZELTME: Tek bir Point nesnesini standart GeoJSON Feature nesnesine çevirir.
        public static object? ToGeoJson(GeoApp.Domain.Entities.Point point)
        {
            if (point == null)
            {
                return null;
            }

            // Karmaþýk Geometry nesnesi yerine, basit ve standart bir geometri nesnesi oluþturuyoruz.
            // Bu, döngüsel referans hatasýný çözer.
            var simpleGeometry = new
            {
                type = "Point",
                coordinates = new[] { point.Geometry.Coordinate.X, point.Geometry.Coordinate.Y }
            };

            return new
            {
                type = "Feature",
                geometry = simpleGeometry,
                properties = new
                {
                    id = point.Id,
                    name = point.Name,
                    description = point.Description,
                    createdByUserId = point.CreatedByUserId
                }
            };
        }

        // DÜZELTME: Point listesini standart GeoJSON FeatureCollection nesnesine çevirir.
        public static object ToGeoJson(IEnumerable<GeoApp.Domain.Entities.Point> points)
        {
            var features = new List<object>();

            if (points != null)
            {
                foreach (var point in points)
                {
                    if (point != null)
                    {
                        // Yukarýdaki ToGeoJson(point) metodunu çaðýrarak her bir noktayý Feature'a çeviriyoruz.
                        features.Add(ToGeoJson(point)!);
                    }
                }
            }

            return new
            {
                type = "FeatureCollection",
                features
            };
        }
    }
}