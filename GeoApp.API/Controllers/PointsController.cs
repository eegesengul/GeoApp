using GeoApp.API.Helpers;
using GeoApp.Application.Features.Points.Commands;
using GeoApp.Application.Features.Points.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GeoApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PointsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/points
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllPointsQuery());
            // Sonucu GeoJsonHelper ile GeoJSON formatına çevirip döndürür.
            return Ok(GeoJsonHelper.ToGeoJson(result));
        }

        // GET: api/points/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var point = await _mediator.Send(new GetPointByIdQuery(id));
            if (point == null)
            {
                return NotFound(); // Nokta bulunamazsa 404 döner.
            }
            return Ok(GeoJsonHelper.ToGeoJson(point));
        }

        // POST: api/points
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePointCommand command)
        {
            var id = await _mediator.Send(command);
            // HTTP 201 Created durum kodu ile yeni kaynağın adresini (GetById) döner.
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // YENİ EKLENEN METOT 1: Güncelleme
        // PUT: api/points/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePointCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Route ID and command ID do not match.");
            }

            await _mediator.Send(command);

            // Başarılı bir güncelleme sonrası standart yanıt HTTP 204 No Content'tir.
            return NoContent();
        }

        // YENİ EKLENEN METOT 2: Silme
        // DELETE: api/points/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeletePointCommand { Id = id });

            // Başarılı bir silme sonrası standart yanıt HTTP 204 No Content'tir.
            return NoContent();
        }
    }
}