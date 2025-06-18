using GeoApp.Application.Features.Areas.Commands;
using GeoApp.Application.Features.Areas.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace GeoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AreasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllAreasQuery());

            var features = result.Select(area => new
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
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateAreaCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id }, new { id });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAreaCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID uyumsuzluğu");

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteAreaCommand { Id = id });
            return NoContent();
        }

        [Authorize]
        [HttpGet("secure")]
        public IActionResult SecureTest()
        {
            return Ok("Token geçerli. Giriş yapmış kullanıcı burayı görebilir.");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("admin-only")]
        public IActionResult AdminTest()
        {
            return Ok("Bu endpoint sadece ADMIN rolündeki kullanıcılar içindir.");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var area = await _mediator.Send(new GetAreaByIdQuery(id));
            return Ok(area);
        }

        [Authorize]
        [HttpGet("by-coordinate")]
        public async Task<IActionResult> GetByCoordinate([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var result = await _mediator.Send(new GetAreaByCoordinateQuery(latitude, longitude));

            if (result == null)
                return NotFound("Bu koordinatla eşleşen bir alan bulunamadı.");

            return Ok(result);
        }


    }
}
