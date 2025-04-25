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
            var events = await _service.GetAllEvents();
            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
        {
            var created = await _service.CreateEvent(request);
            return Ok(new BaseResponseEmpty
            {
                Message = "Event successfully created!"
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventResponse>> GetById([FromRoute] Guid id)
        {
            var events = await _service.GetAllEvents();
            var ev = events.FirstOrDefault(e => e.Id == id);
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
