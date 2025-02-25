using Amazon.S3.Model.Internal.MarshallTransformations;
using Infrastructure.Exceptions;
using MyBackedApi.DTOs.User.Requests;
using MyBackedApi.DTOs.User.Responses;
using MyBackedApi.Enums;
using MyBackedApi.Models;
using MyBackedApi.Repositories;
using MyBackendApi.Models.Responses;
using MyBackendApi.Repositories;

namespace MyBackendApi.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly OccupationRepository _occupationRepository;

        public UserService(
            UserRepository userRepository,
            OccupationRepository occupationRepository)
        {
            _userRepository = userRepository;
            _occupationRepository = occupationRepository;
        }

        public async Task<List<GetUserResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(user => new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role,
                CompletedSteps = user.CompletedSteps,
                OccupationName = user.Occupation.Name,
                IsApproved = user.IsApproved
            }).ToList();
        }

        public async Task<GetUsersResponse> GetMentorsAsync(GetMentorsRequest payload)
        {
            var usersResponse = await _userRepository.GetUsersWithOccupation(payload);
            var mentors = usersResponse.Mentors.Select(user => new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role,
                CompletedSteps = user.CompletedSteps,
                OccupationName = user.Occupation.Name,
                IsApproved = user.IsApproved
            })
                .OrderBy(u => u.IsApproved)
                .ToList();


            return new()
            {
                Users = mentors,
                TotalUsers = usersResponse.TotalUsers
            };
        }

        public async Task<GetUsersResponse> GetAdminsCorporateAsync(GetAdminsCorporateRequest payload)
        {
            var usersResponse = await _userRepository.GetAdminsCorporateAsync(payload);
            var admins = usersResponse.Admins.Select(user => new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role,
                CompletedSteps = user.CompletedSteps,
                OccupationName = user.Occupation.Name,
                IsApproved = user.IsApproved
            }).ToList();

            return new()
            {
                Users = admins,
                TotalUsers = usersResponse.TotalUsers
            };
        }

        public async Task<GetUserResponse> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role,
                CompletedSteps = user.CompletedSteps,
                OccupationName = user.Occupation.Name,
                IsApproved = user.IsApproved
            };
        }

        public async Task AddUserAsync(AddUserRequest user)
        {
            var userEntity = new User
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Password = user.Password,
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

        public async Task<AddOccupationResponse> AddOccupationAsync(AddOccupationRequest occupation)
        {
            if (occupation.Domain != occupation.AdminEmail.Substring(occupation.AdminEmail.Length - occupation.Domain.Length))
                throw new OperationNotAllowedException("The email doesn't match the desired behaviour!");

            var user = await _userRepository.GetUserByEmailAsync(occupation.AdminEmail);
            if (user != null)
                throw new OperationNotAllowedException("This email is already associated with another account!");

            var occupationEntity = new Occupation
            {
                Name = occupation.Name,
                Domain = occupation.Domain,
                AdminEmail = occupation.AdminEmail,
                AdminName = occupation.AdminName,
                AdminSurname = occupation.AdminSurname
            };

            var occupationId = await _occupationRepository.AddOccupationAsync(occupationEntity);

            var newUser = new User
            {
                Name = occupation.AdminName,
                Surname = occupation.AdminSurname,
                Email = occupation.AdminEmail,
                Role = RoleTypeEnum.ADMIN_CORPORATE,
                Password = BCrypt.Net.BCrypt.HashPassword(occupation.AdminEmail, 10),
                IsApproved = true,
                OccupationId = occupationId
            };

            await _userRepository.AddUserAsync(newUser);

            return new()
            {
                Id = occupationId
            };
        }
        public async Task UpdateOccupationAsync(UpdateOccupationRequest request)
        {
            var existingOccupation = await _occupationRepository.GetOccupationByIdAsync(request.OccupationId);
            if (existingOccupation == null)
            {
                throw new KeyNotFoundException("Company not found.");
            }
            if(request.AdminSurname != existingOccupation.AdminSurname || request.AdminName != existingOccupation.AdminName)
            {
                var adminId = await _occupationRepository.GetAdminForOccupation(request.OccupationId);
                var updatedUser = new UpdateUserRequest
                {
                    Id = adminId,
                    Name = request.AdminName,
                    Surname = request.AdminSurname
                };

                await UpdateUserAsync(updatedUser); 
            }

            existingOccupation.Name = request.Name;
            existingOccupation.Address = request.Address;
            existingOccupation.City = request.City;
            existingOccupation.Country = request.Country;
            existingOccupation.AdminName = request.AdminName;
            existingOccupation.AdminSurname = request.AdminSurname;

            await _occupationRepository.UpdateOccupationAsync(existingOccupation);
        }

        public async Task ApproveUserAsync(Guid userId)
        {
            await _userRepository.ApproveUserAsync(userId);
        }

        public async Task<GetCurrentUserDetailsResponse> GetCurrentUserDetailsAsync(Guid userId)
        {
            var currentUser = await _userRepository.GetUserByIdAsync(userId);

            if (currentUser == null)
                throw new ResourceMissingException(("The desired user does not exist"));

            var result = new GetCurrentUserDetailsResponse()
            {
                Id = currentUser.Id,
                Email = currentUser.Email,
                Name = currentUser.Name,
                Surname = currentUser.Surname,
                Role = currentUser.Role,
                OccupationId = currentUser.OccupationId,
                Occupation = currentUser.Occupation
            };

            return result;

        }

        public async Task<GetMyCompanyResponse> GetCompanyAsync(Guid userId)
        {
            var currentUser = await _userRepository.GetUserByIdAsync(userId);
            var occupation = await _occupationRepository.GetOccupationByIdAsync(currentUser.OccupationId);

            var numberResponses = await _occupationRepository.GetNumberOfResponsesAsync(currentUser.OccupationId);
            var numberUsers = await _occupationRepository.GetNumberOfUsersAsync(currentUser.OccupationId);

            return new GetMyCompanyResponse
            {
                Id = occupation.Id,
                Name = occupation.Name,
                Address = occupation.Address,
                City = occupation.City,
                Country = occupation.Country,
                Domain = occupation.Domain,
                TotalResponses = numberResponses,
                TotalUsers = numberUsers,
                AdminEmail = occupation.AdminEmail,
                AdminName = occupation.AdminName,
                AdminSurname = occupation.AdminSurname
            };
        }

        public async Task<GetUsersResponse> GetUsersForRoleAsync(GetUsersForRoleRequest payload)
        {
            var usersResponse = await _userRepository.GetUsersByRole(payload);
            var users = usersResponse.Users.Select(user => new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role,
                CompletedSteps = user.CompletedSteps,
                OccupationName = user.Occupation.Name,
                IsApproved = user.IsApproved
            }).ToList();


            return new()
            {
                Users = users,
                TotalUsers = usersResponse.TotalUsers
            };
        }
    }
}
