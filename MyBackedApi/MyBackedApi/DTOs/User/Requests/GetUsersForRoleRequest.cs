using MyBackedApi.Enums;

namespace MyBackedApi.DTOs.User.Requests
{
    public class GetUsersForRoleRequest
    {
        public RoleTypeEnum Role { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; }
    }
}
