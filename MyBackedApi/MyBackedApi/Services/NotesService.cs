using MyBackedApi.DTOs;
using MyBackedApi.Models;
using MyBackedApi.Repositories;

namespace MyBackedApi.Services
{
    public class NotesService
    {
        private readonly NotesRepository _repo;

        public NotesService(NotesRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<NoteDto>> GetNotesForStudent(Guid studentId)
        {
            var notes = await _repo.GetAllAsync(studentId);
            return notes.Select(n => new NoteDto
            {
                Id = n.Id,
                Title = n.Title,
                Color = n.Color,
                Datef = n.Datef
            }).ToList();
        }

        public async Task<NoteDto> CreateNote(CreateNoteDto dto, Guid studentId)
        {
            var note = new Note
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Color = dto.Color,
                Datef = dto.Datef,
                StudentId = studentId
            };
            await _repo.AddAsync(note);
            return new NoteDto { Id = note.Id, Title = note.Title, Color = note.Color, Datef = note.Datef };
        }

        public async Task DeleteNote(Guid id) => await _repo.DeleteAsync(id);

        public async Task<NoteDto> UpdateNote(NoteDto dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id); // adaugă acest method în repo dacă nu-l ai
            if (existing == null) throw new Exception("Note not found");

            existing.Title = dto.Title;
            existing.Color = dto.Color;
            existing.Datef = dto.Datef;

            var updated = await _repo.UpdateAsync(existing);
            return new NoteDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Color = updated.Color,
                Datef = updated.Datef
            };
        }

    }
}
