using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.DTOs.Forum.Requests;
using MyBackedApi.Enums;
using MyBackedApi.Models;

namespace MyBackendApi.Repositories
{
    public class QuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync(GetQuestionsRequest payload)
        {
            var query = _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.User)
                    .ThenInclude(u => u.Occupation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(payload.Search))
            {
                query = query.Where(q => q.Title.Contains(payload.Search) || q.Content.Contains(payload.Search));
            }

            if (payload.Topic != null)
            {
                query = query.Where(q => q.Topic == payload.Topic);
            }

            var skip = (payload.Page - 1) * payload.PageSize;
            query = query.Skip(skip).Take(payload.PageSize);

            return await query.ToListAsync();
        }


        public async Task<Question?> GetQuestionByIdAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}   
