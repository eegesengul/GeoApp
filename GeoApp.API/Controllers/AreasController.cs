using GeoApp.API.Dtos;
using GeoApp.Domain.Entities;
using GeoApp.Infrastructure.Persistence; // ✅ Doğru namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace GeoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AreasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var areas = await _context.Areas.ToListAsync();

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

            var geoJson = new
            {
                type = "FeatureCollection",
                features = features
            };

            return Ok(geoJson);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAreaDto dto)
        {
            var reader = new WKTReader();
            Geometry geometry;

            try
            {
                geometry = reader.Read(dto.WKTGeometry);
            }
            catch
            {
                return BadRequest("Geçersiz WKT geometrisi.");
            }

            var area = new Area
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Geometry = geometry
            };

            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = area.Id }, new
            {
                area.Id,
                area.Name,
                area.Description
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateAreaDto dto)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }

            var reader = new WKTReader();
            Geometry geometry;

            try
            {
                geometry = reader.Read(dto.WKTGeometry);
            }
            catch
            {
                return BadRequest("Geçersiz geometri.");
            }

            area.Name = dto.Name;
            area.Description = dto.Description;
            area.Geometry = geometry;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
