using MyBackedApi.DTOs.Calendar.Requests;
using MyBackedApi.DTOs.Calendar.Responses;
using MyBackedApi.Models;
using MyBackedApi.Repositories;

namespace MyBackedApi.Services
{
    public class EventsService
    {
        private readonly EventsRepository _repo;

        public EventsService(EventsRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<EventResponse>> GetAllEvents()
        {
            var events = await _repo.GetAllAsync();
            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Title = e.Title,
                ColorPrimary = e.ColorPrimary,
                Start = e.Start,
                End = e.End
            }).ToList();
        }

        public async Task<EventResponse> CreateEvent(CreateEventRequest dto)
        {
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                ColorPrimary = dto.ColorPrimary,
                Start = dto.Start,
                End = dto.End
            };
            await _repo.AddAsync(ev);
            return new EventResponse
            {
                Id = ev.Id,
                Title = ev.Title,
                ColorPrimary = ev.ColorPrimary,
                Start = ev.Start,
                End = ev.End
            };
        }

        public async Task DeleteEvent(Guid id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<EventResponse> UpdateEvent(EventResponse dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null)
                throw new UnauthorizedAccessException("Event not found");

            existing.Title = dto.Title;
            existing.ColorPrimary = dto.ColorPrimary;
            existing.Start = dto.Start;
            existing.End = dto.End;

            var updated = await _repo.UpdateAsync(existing);
            return new EventResponse
            {
                Id = updated.Id,
                Title = updated.Title,
                ColorPrimary = updated.ColorPrimary,
                Start = updated.Start,
                End = updated.End
            };
        }
    }
}
