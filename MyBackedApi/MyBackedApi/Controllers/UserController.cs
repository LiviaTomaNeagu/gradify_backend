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

        [HttpPost("get-mentors")]
        public async Task<GetMentorsResponse> GetMentors([FromBody]GetMentorsRequest payload)
        {
            return await _userService.GetMentorsAsync(payload);
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
        public async Task<IActionResult> AddOccupation([FromBody] AddOccupationRequest request)
        {
            await _userService.AddOccupationAsync(request);
            return Ok("Success!");
        }

        [HttpPut("approve-user")]
        public async Task<IActionResult> ApproveUser([FromBody] Guid userId)
        {
            await _userService.ApproveUserAsync(userId);
            return Ok("Success!");
        }

        [HttpDelete("decline-user")]
        public async Task<IActionResult> DeclineUser([FromBody] Guid userId)
        {
            await _userService.DeleteUserAsync(userId);
            return Ok("User not approved!");
        }
    }
}
