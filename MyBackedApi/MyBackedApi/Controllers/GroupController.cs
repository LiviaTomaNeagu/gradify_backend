using Infrastructure.Base;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.Services;
using MyBackedApi.Models;
using MyBackedApi.DTOs;

namespace MyBackedApi.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupController : BaseApiController
    {
        private readonly GroupService _service;

        public GroupController(GroupService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllGroups()
        {
            var groups = await _service.GetAllGroupsAsync();
            return Ok(groups);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Group name is required.");

            var group = await _service.CreateGroupAsync(request.Name);
            return Ok(group);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            var success = await _service.DeleteGroupAsync(request.Id);
            if (!success)
                return NotFound("Group not found.");

            return Ok();
        }
    }

}
