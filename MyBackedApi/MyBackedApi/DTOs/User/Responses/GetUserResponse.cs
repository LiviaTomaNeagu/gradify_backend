using MyBackedApi.Enums;
using MyBackedApi.Models;

namespace MyBackendApi.Models.Responses
{
    public class GetUserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public RoleTypeEnum Role { get; set; }
        public int CompletedSteps { get; set; }
        public string OccupationName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; }
        public GetStudentDetailsResponse? StudentDetails { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class GetStudentDetailsResponse
    {
        public string Faculty { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
    }
}