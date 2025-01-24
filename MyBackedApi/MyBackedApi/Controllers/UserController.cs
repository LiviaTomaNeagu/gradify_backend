using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.User.Requests;
using MyBackendApi.Models.Responses;
using MyBackendApi.Services;

namespace MyBackendApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
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
    }
}
