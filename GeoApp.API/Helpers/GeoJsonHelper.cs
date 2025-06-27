using GeoApp.Application.Dtos;
using System.Collections.Generic;
using System.Linq;

// D�KKAT: NetTopologySuite using'leri art�k do�rudan burada kullan�lm�yor,
// ��nk� her �eyi standart nesnelere �eviriyoruz.

namespace GeoApp.API.Helpers
{
    public static class GeoJsonHelper
    {
        // --- Area Metotlar� (De�i�iklik yok) ---
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

        // --- Point Metotlar� (TAMAMEN YEN�DEN YAZILDI) ---

        // D�ZELTME: Tek bir Point nesnesini standart GeoJSON Feature nesnesine �evirir.
        public static object? ToGeoJson(GeoApp.Domain.Entities.Point point)
        {
            if (point == null)
            {
                return null;
            }

            // Karma��k Geometry nesnesi yerine, basit ve standart bir geometri nesnesi olu�turuyoruz.
            // Bu, d�ng�sel referans hatas�n� ��zer.
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

        // D�ZELTME: Point listesini standart GeoJSON FeatureCollection nesnesine �evirir.
        public static object ToGeoJson(IEnumerable<GeoApp.Domain.Entities.Point> points)
        {
            var features = new List<object>();

            if (points != null)
            {
                foreach (var point in points)
                {
                    if (point != null)
                    {
                        // Yukar�daki ToGeoJson(point) metodunu �a��rarak her bir noktay� Feature'a �eviriyoruz.
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