using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.Models;
using MyBackendApi.Models;
using System;

namespace MyBackendApi.Repositories
{
    public class AnswerRepository
    {
        private readonly ApplicationDbContext _context;

        public AnswerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAnswerAsync(Answer answer)
        {
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetAnswersCountForUser(Guid userId)
        {
            return await _context.Answers
                .Where(a => a.UserId == userId)
                .CountAsync();
        }

        public async Task<List<Answer>> GetLastWeekAnswers(Guid userId)
        {
            return await _context.Answers
                .Where(a => a.UserId == userId && a.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .ToListAsync();
        }
    }
}
