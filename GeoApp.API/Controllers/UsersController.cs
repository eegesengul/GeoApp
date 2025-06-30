using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GeoApp.Application.Features.Users.Commands;
using GeoApp.Application.Features.Users.Queries;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ADMIN işlemleri (tüm kullanıcılar üzerinde işlem yapma)
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _mediator.Send(new GetUsersQuery());
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id mismatch.");
            var updated = await _mediator.Send(command);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var deleted = await _mediator.Send(new DeleteUserCommand { Id = id });
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        // Giriş yapmış HER KULLANICI için kendi bilgileri (profil)
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            // Doğru claimden al
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _mediator.Send(new GetMyUserQuery { UserId = userId });
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserCommand command)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            if (userId != command.Id)
                return Forbid();

            var updated = await _mediator.Send(command);
            if (!updated)
                return NotFound();
            return NoContent();
        }
    }
}