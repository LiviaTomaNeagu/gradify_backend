using MyBackedApi.DTOs.User;
using MyBackedApi.Enums;
using MyBackedApi.Models;
using MyBackendApi.Models.Responses;

namespace MyBackendApi.Mappings
{
    public static class UserMappingExtensions
    {
        public static ShortUserDto ToShortUserDto(this User user)
        {
            return new ShortUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role,
                IsApproved = user.IsApproved,
                Occupation = user.Occupation,
                OccupationId = user.OccupationId
            };
        }

        public static List<ShortUserDto> ToShortUsersDto(this List<User> users)
        {
            return users.Select(u => u.ToShortUserDto()).ToList();
        }

        public static GetStudentDetailsResponse ToGetStudentDetailsResponse(this StudentDetails studentDetails)
        {
            return new GetStudentDetailsResponse
            {
                Faculty = studentDetails.Faculty,
                Specialization = studentDetails.Specialization,
                Group = studentDetails.Group,
                EnrollmentDate = studentDetails.EnrollmentDate
            };
        }
    }
}