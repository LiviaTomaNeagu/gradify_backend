﻿using Amazon.S3.Model.Internal.MarshallTransformations;
using Infrastructure.Exceptions;
using MyBackedApi.DTOs.User;
using MyBackedApi.DTOs.User.Requests;
using MyBackedApi.DTOs.User.Responses;
using MyBackedApi.Enums;
using MyBackedApi.Models;
using MyBackedApi.Repositories;
using MyBackendApi.Models.Responses;
using MyBackendApi.Repositories;
using MyBackendApi.Mappings;
using MyBackedApi.Mappings;

namespace MyBackendApi.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly OccupationRepository _occupationRepository;
        public readonly AnswerRepository _answerRepository;
        public readonly QuestionRepository _questionRepository;

        public UserService(
            UserRepository userRepository,
            OccupationRepository occupationRepository,
            AnswerRepository answerRepository,
            QuestionRepository questionRepository)
        {
            _userRepository = userRepository;
            _occupationRepository = occupationRepository;
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
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
                CreatedAt = user.CreatedAt,
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
                CreatedAt = user.CreatedAt,
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
                CreatedAt = user.CreatedAt,
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
                CreatedAt = user.CreatedAt,
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
                CreatedAt = user.CreatedAt,
                IsApproved = user.IsApproved
            }).ToList();


            return new()
            {
                Users = users,
                TotalUsers = usersResponse.TotalUsers,
                FilteredUsers = usersResponse.FilteredUsers
            };
        }

        public async Task<GetOccupationsResponse> GetOccupationsAsync(GetOccupationsRequest payload)
        {
            var occupationsResponse = await _occupationRepository.GetOccupations(payload);

            var occupations = new List<ShortOccupationDTO>();

            foreach (var o in occupationsResponse.Occupations)
            {
                var isActive = await _occupationRepository.IsOccupationActive(o.Id); // Run sequentially

                occupations.Add(new ShortOccupationDTO
                {
                    Id = o.Id,
                    Name = o.Name,
                    AdminEmail = o.AdminEmail,
                    AdminSurname = o.AdminSurname,
                    AdminName = o.AdminName,
                    IsActive = isActive
                });
            }

            return new GetOccupationsResponse
            {
                Occupations = occupations,
                TotalNumber = occupationsResponse.TotalOccupation,
                TotalActive = await _occupationRepository.GetActiveOccupationsCount()
            };
        }

        public async Task<GetOccupationDetailsResponse> GetOccupationDetailAsync(Guid occupationId)
        {
            var occupation = await _occupationRepository.GetOccupationByIdAsync(occupationId);
            return new GetOccupationDetailsResponse()
            {
                Id = occupationId,
                Name = occupation.Name,
                Domain = occupation.Domain,
                Address = occupation.Address,
                City = occupation.City,
                Country = occupation.Country,
                AdminEmail = occupation.AdminEmail,
                AdminName = occupation.AdminName,
                AdminSurname = occupation.AdminSurname,
                CreatedAt =occupation.CreatedAt,
                IsActive = await _occupationRepository.IsOccupationActive(occupationId),
                NumberOfResponses = await _occupationRepository.GetNumberOfResponsesAsync(occupationId)
            };
        }

        public async Task<GetStatsMentorResponse> GetStatsForMentorAsync(Guid mentorId)
        {
            var answersCount = await _answerRepository.GetAnswersCountForUser(mentorId);
            var topUsers = await _questionRepository.GetUsersInteractedWith(mentorId);
            var latestQuestions = await _questionRepository.GetLatestQuestions(mentorId);
            var lastWeekAnswers = await _answerRepository.GetLastWeekAnswers(mentorId);
            var favoriteTopics = await _questionRepository.GetFavoriteTopic(mentorId);

            var activityGraph = GenerateActivityGraph(lastWeekAnswers);

            return new GetStatsMentorResponse
            {
                TopUsers = topUsers.ToShortUsersDto(),
                TotalAnswers = answersCount,
                LatestQuestions = latestQuestions.ToLatestQuestions(),
                ActivityGraph = activityGraph,
                FavoriteTopics = favoriteTopics
            };
        }

        private List<GraphDataPoint> GenerateActivityGraph(List<Answer> lastWeekAnswers)
        {
            var today = DateTime.UtcNow.Date;
            var startOfWeek = today.AddDays(-6);

            var graphData = new List<GraphDataPoint>();

            for (int i = 0; i < 7; i++)
            {
                var day = startOfWeek.AddDays(i);
                graphData.Add(new GraphDataPoint
                {
                    Name = day.ToString("dddd"),
                    Value = lastWeekAnswers.Count(a => a.CreatedAt.Date == day)
                });
            }

            return graphData;
        }

    }
}
