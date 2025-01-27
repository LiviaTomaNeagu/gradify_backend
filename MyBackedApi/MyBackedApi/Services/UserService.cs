using MyBackedApi.DTOs.User.Requests;
using MyBackedApi.Models;
using MyBackendApi.Models.Responses;
using MyBackendApi.Repositories;

namespace MyBackendApi.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<GetUserResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(user => new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CompletedSteps = user.CompletedSteps,
                OccupationName = user.Occupation.Name
            }).ToList();
        }

        public async Task<GetUserResponse> GetUserByIdAsync(Guid id)
        {
            var user =  await _userRepository.GetUserByIdAsync(id);
            return new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CompletedSteps = user.CompletedSteps,
                OccupationName = user.Occupation.Name
            };
        }

        public async Task AddUserAsync(AddUserRequest user)
        {
            var userEntity = new User
            {
                Username = user.Username,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PasswordHash = user.Password,
                Role = user.Role,
                OccupationId = user.OccupationId
            };

            if (string.IsNullOrEmpty(user.Name))
            {
                throw new ArgumentException("User name is required.");
            }

            await _userRepository.AddUserAsync(userEntity);
        }

        public async Task UpdateUserAsync(UpdateUserRequest user)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            existingUser.Name = user.Name;
            existingUser.Surname = user.Surname;
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;

            await _userRepository.UpdateUserAsync(existingUser);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            await _userRepository.DeleteUserAsync(id);
        }

        public async Task AddOccupationAsync(AddOccupationRequest occupation)
        {
            var occupationEntity = new Occupation
            {
                Name = occupation.Name
            };

            await _userRepository.AddOccupationAsync(occupationEntity);
        }
    }
}
