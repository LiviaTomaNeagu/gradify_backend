using MyBackedApi.Data;
using MyBackendApi.Models;
using Microsoft.EntityFrameworkCore;
using MyBackendApi.Repositories;
using SendGrid.Helpers.Mail;
using MyBackedApi.Models;

namespace MyBackedApi.Repositories
{
    public class GroupRepository : BaseRepository
    {
        public GroupRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Group>> GetAllGroupsAsync()
        {
            return await _context.Groups.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<bool> DeleteGroupAsync(Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null) return false;

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
