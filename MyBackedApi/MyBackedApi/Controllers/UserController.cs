using Infrastructure.Base;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.User.Requests;
using MyBackedApi.DTOs.User.Responses;
using MyBackendApi.Models.Responses;
using MyBackendApi.Services;

namespace MyBackendApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : BaseApiController
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-user/{id}")]
        public async Task<GetUserResponse?> GetUserById([FromRoute] Guid id)
        {
            var response = await _userService.GetUserByIdAsync(id);

            return response;
        }

        [HttpGet("get-all-users")]
        public async Task<List<GetUserResponse>> GetAllUsers()
        {
            return await _userService.GetAllUsersAsync();
        }

        [HttpGet("get-all-short-users")]
        public async Task<GetShortUsersResponse> GetAllShortUsers()
        {
            var currentUserId = GetUserIdFromToken();
            return await _userService.GetAllShortUsersAsync(currentUserId);
        }

        [HttpPost("get-users-for-role")]
        public async Task<GetUsersResponse> GetUsersForRole([FromBody] GetUsersForRoleRequest payload)
        {
            return await _userService.GetUsersForRoleAsync(payload);
        }

        [HttpPost("get-mentors")]
        public async Task<GetUsersResponse> GetMentors([FromBody] GetMentorsRequest payload)
        {
            var currentUserId = GetUserIdFromToken();
            return await _userService.GetMentorsAsync(payload, currentUserId);
        }

        [HttpPost("get-admins-corporate")]
        public async Task<GetUsersResponse> GetAdminsCorporate([FromBody] GetAdminsCorporateRequest payload)
        {
            return await _userService.GetAdminsCorporateAsync(payload);
        }

        [HttpPost("get-companies")]
        public async Task<GetOccupationsResponse> GetOccupations([FromBody] GetOccupationsRequest payload)
        {
            return await _userService.GetOccupationsAsync(payload);
        }

        [HttpGet("get-company-details/{occupationId}")]
        public async Task<GetOccupationDetailsResponse> GetOccupationDetails([FromRoute] Guid occupationId)
        {
            return await _userService.GetOccupationDetailAsync(occupationId);
        }


        [HttpGet("get-current-user-details")]
        public async Task<ActionResult<GetCurrentUserDetailsResponse>> GetCurrentUserDetails()
        {
            var userId = GetUserIdFromToken();

            var results = await _userService.GetCurrentUserDetailsAsync(userId);

            return Ok(results);

        }

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
        {
            await _userService.AddUserAsync(request);
            return Ok("Success!");
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            await _userService.UpdateUserAsync(request);
            return Ok("Success!");
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok("Success!");
        }

        [HttpPost("add-occupation")]
        public async Task<AddOccupationResponse> AddOccupation([FromBody] AddOccupationRequest request)
        {
            return await _userService.AddOccupationAsync(request);
        }

        [HttpPut("update-occupation")]
        public async Task<IActionResult> UpdateOccupation([FromBody] UpdateOccupationRequest request)
        {
            await _userService.UpdateOccupationAsync(request);
            return Ok(new BaseResponseEmpty()
            {
                Message = "Success!"
            });
        }

        [HttpPut("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser([FromRoute] Guid userId)
        {
            await _userService.ApproveUserAsync(userId);
            return Ok(new BaseResponseEmpty()
            {
                Message = "Success!"
            });
        }

        [HttpDelete("decline-user/{userId}")]
        public async Task<IActionResult> DeclineUser([FromRoute] Guid userId)
        {
            await _userService.DeleteUserAsync(userId);
            return Ok(new BaseResponseEmpty()
            {
                Message = "User declined!"
            });
        }

        [HttpGet("get-my-company")]
        public async Task<ActionResult<GetMyCompanyResponse>> GetMyCompany()
        {
            var userId = GetUserIdFromToken();
            var response = await _userService.GetCompanyAsync(userId);
            return Ok(response);
        }

        [HttpGet("stats/{mentorId}")]
        public async Task<ActionResult<GetStatsMentorResponse>> GetStatsForMentor([FromRoute] Guid mentorId)
        {
            var response = await _userService.GetStatsForMentorAsync(mentorId);
            return Ok(response);
        }

        [HttpPost("add-coordinator-for/{userId}")]
        public async Task<IActionResult> AddCoodinatorForStudent([FromRoute] Guid userId)
        {
            var currentUserId = GetUserIdFromToken();
            await _userService.AddCoodinatorForStudentAsync(userId, currentUserId);
            return Ok(new BaseResponseEmpty()
            {
                Message = "Student added!"
            });
        }

        [HttpGet("has-student-details")]
        public async Task<ActionResult<HasDetailsResponse>> HasStudentDetailsAsync()
        {
            var currentUserId = GetUserIdFromToken();
            var response = await _userService.HasStudentDetailsAsync(currentUserId);
            return Ok(response);
        }

        [HttpPost("add-student-details")]
        public async Task<IActionResult> AddStudentDetailsAsync([FromBody] AddStudentDetails addStudentDetails)
        {
            var currentUserId = GetUserIdFromToken();
            await _userService.AddStudentDetailsAsync(addStudentDetails, currentUserId);

            return Ok(new BaseResponseEmpty()
            {
                Message = "Details added!"
            });
        }

        [HttpGet("get-account-details")]
        public async Task<ActionResult<GetAccountDetailsResponse>> GetAccountDetails()
        {
            var currentUserId = GetUserIdFromToken();
            var response = await _userService.GetAccountDetailsAsync(currentUserId);
            return Ok(response);
        }

        [HttpPost("add-topic")]
        public async Task<IActionResult> AddTopic([FromBody] AddTopicRequest payload)
        {
            var currentUserId = GetUserIdFromToken();
            await _userService.AddTopicForUserAsync(payload, currentUserId);
            return Ok(new BaseResponseEmpty()
            {
                Message = "Topic added!"
            });
        }


    }
}
