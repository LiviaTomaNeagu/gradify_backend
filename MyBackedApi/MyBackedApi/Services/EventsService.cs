using MyBackedApi.DTOs.Calendar.Requests;
using MyBackedApi.DTOs.Calendar.Responses;
using MyBackedApi.Models;
using MyBackedApi.Repositories;
using MyBackendApi.Repositories;

namespace MyBackedApi.Services
{
    public class EventsService
    {
        private readonly EventsRepository _repo;
        private readonly UserRepository _userRepo;

        public EventsService(EventsRepository repo,
            UserRepository userRepository)
        {
            _repo = repo;
            _userRepo = userRepository;
        }

        public async Task<List<EventResponse>> GetAllEvents(Guid currentUserId)
        {
            var user = await _userRepo.GetUserByIdAsync(currentUserId);


            if(user.Role == Enums.RoleTypeEnum.STUDENT)
            {
                Guid coordinatorId = await _userRepo.GetCoordinatorForStudent(currentUserId);
                var studentEvents = await _repo.GetEventsByCoordinatorIdAsync(coordinatorId);
                return studentEvents.Select(e => new EventResponse
                {
                    Id = e.Id,
                    Title = e.Title,
                    ColorPrimary = e.ColorPrimary,
                    Start = e.Start,
                    End = e.End,
                    CoordinatorId = e.CoordinatorId
                }).ToList();
            }

            var events = await _repo.GetEventsByCoordinatorIdAsync(currentUserId);
            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Title = e.Title,
                ColorPrimary = e.ColorPrimary,
                Start = e.Start,
                End = e.End,
                CoordinatorId = e.CoordinatorId
            }).ToList();
        }

        public async Task<EventResponse> CreateEvent(CreateEventRequest dto, Guid currentUserId)
        {
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                ColorPrimary = dto.ColorPrimary,
                Start = dto.Start,
                End = dto.End,
                CoordinatorId = currentUserId
            };
            await _repo.AddAsync(ev);
            return new EventResponse
            {
                Id = ev.Id,
                Title = ev.Title,
                ColorPrimary = ev.ColorPrimary,
                Start = ev.Start,
                End = ev.End,
                CoordinatorId = currentUserId
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
                End = updated.End,
                CoordinatorId = existing.CoordinatorId
            };
        }

        public async Task<Event> GetEventById(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }
    }
}
