
using MyBackedApi.Data;
using MyBackedApi.Models;
using MyBackendApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyBackedApi.Repositories
{
    public class NotesRepository : BaseRepository
    {
        public NotesRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Note>> GetAllAsync(Guid studentId) =>
            await _context.Notes.Where(n => n.StudentId == studentId).ToListAsync();

        public async Task<Note> AddAsync(Note note)
        {
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task DeleteAsync(Guid id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Note> UpdateAsync(Note note)
        {
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<Note> GetByIdAsync(Guid id)
        {
            return await _context.Notes.FindAsync(id);
        }

    }
}
