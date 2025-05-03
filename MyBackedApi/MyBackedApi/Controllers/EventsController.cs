using Infrastructure.Base;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.Calendar.Requests;
using MyBackedApi.DTOs.Calendar.Responses;
using MyBackedApi.Services;

namespace MyBackedApi.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : BaseApiController
    {
        private readonly EventsService _service;

        public EventsController(EventsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventResponse>>> GetAll()
        {
            var currentUserId = GetUserIdFromToken();
            var events = await _service.GetAllEvents(currentUserId);
            return Ok(events);
        }

        [HttpPost]
        public async Task<ActionResult<EventResponse>> Create([FromBody] CreateEventRequest request)
        {
            var currentUserId = GetUserIdFromToken();
            var created = await _service.CreateEvent(request, currentUserId);
            return Ok(created);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventResponse>> GetById([FromRoute] Guid id)
        {
            var ev = await _service.GetEventById(id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EventResponse>> Update([FromRoute] Guid id, [FromBody] EventResponse updatedEvent)
        {
            if (id != updatedEvent.Id)
                return BadRequest("ID mismatch");

            var updated = await _service.UpdateEvent(updatedEvent);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _service.DeleteEvent(id);
            return Ok(new BaseResponseEmpty()
            {
                Message = "Event deleted!"
            });
        }
    }
}
