using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using MyBackedApi.Data;
using MyBackedApi.Enums;
using MyBackedApi.Models;
using MyBackendApi.Repositories;

namespace MyBackedApi.Repositories
{
    public class UserTopicsRepository : BaseRepository
    {
        public UserTopicsRepository(ApplicationDbContext context) : base(context)
        { }
        public async Task AddTopicForUserAsync(TopicEnum topic, Guid currentUserId)
        {
            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null)
            {
                throw new WrongInputException("User not found.");
            }

            bool topicExists = await _context.UsersTopics
                .AnyAsync(ut => ut.UserId == currentUserId && ut.Topic == topic);

            if (topicExists)
            {
                throw new OperationNotAllowedException("User already has this topic.");
            }

            var userTopic = new UserTopics
            {
                UserId = currentUserId,
                Topic = topic
            };

            _context.UsersTopics.Add(userTopic);
            await _context.SaveChangesAsync();
        }

    }
}
