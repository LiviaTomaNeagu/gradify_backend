using MyBackedApi.Models;
using MyBackedApi.Repositories;

namespace MyBackedApi.Services
{
    public class GroupService
    {
        private readonly GroupRepository _repo;

        public GroupService(GroupRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Group>> GetAllGroupsAsync()
        {
            return await _repo.GetAllGroupsAsync();
        }

        public async Task<Group> CreateGroupAsync(string name)
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            return await _repo.CreateGroupAsync(group);
        }

        public async Task<bool> DeleteGroupAsync(Guid groupId)
        {
            return await _repo.DeleteGroupAsync(groupId);
        }
    }
}
