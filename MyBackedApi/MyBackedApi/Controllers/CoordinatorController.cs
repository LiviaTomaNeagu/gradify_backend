using Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.Coordinator.Responses;
using MyBackedApi.DTOs.User;
using MyBackedApi.DTOs.User.Requests;
using MyBackedApi.DTOs.User.Responses;
using MyBackendApi.Models.Responses;
using MyBackendApi.Services;

namespace MyBackedApi.Controllers
{
    [Authorize(Roles = "COORDINATOR")]
    [ApiController]
    [Route("api/coordinator")]
    public class CoordinatorController : BaseApiController
    {
        private readonly UserService _userService;

        public CoordinatorController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-available-students")]
        public async Task<GetAvailableStudentsResponse> GetAvailableStudents()
        {
            var response = await _userService.GetAvailableStudentsAsync();

            return response;
        }

        [HttpGet("get-student/{studentId}")]
        public async Task<GetStudentResponse> GetStudent([FromRoute] Guid studentId)
        {
            var response = await _userService.GetStudentAsync(studentId);

            return response;
        }

        [HttpPost("get-my-students")]
        public async Task<GetUsersResponse> GetMyStudents(GetUsersForRoleRequest payload)
        {
            var currentUserId = GetUserIdFromToken();
            var response = await _userService.GetMyStudentsAsync(payload, currentUserId);

            return response;
        }

        [HttpGet("add-my-student/{studentId}")]
        public async Task<IActionResult> AddMyStudent([FromRoute] Guid studentId)
        {
            var coordinatorId = GetUserIdFromToken();
            await _userService.AddMyStudentAsync(studentId, coordinatorId);

            return Ok(new BaseResponseEmpty()
            {
                Message = "Student added successfully!"
            });
        }

        [HttpGet("remove-my-student/{studentId}")]
        public async Task<IActionResult> RemoveMyStudent([FromRoute] Guid studentId)
        {
            var coordinatorId = GetUserIdFromToken();
            await _userService.RemoveMyStudentAsync(studentId, coordinatorId);

            return Ok(new BaseResponseEmpty()
            {
                Message = "Student removed successfully!"
            });
        }
    }
}
