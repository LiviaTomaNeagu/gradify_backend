using Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs;
using MyBackedApi.DTOs.Notifications.Requests;
using MyBackedApi.Models;
using MyBackedApi.Services;

namespace MyBackedApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : BaseApiController
    {
        private readonly NotificationsService _service;

        public NotificationsController(NotificationsService service)
        {
            _service = service;
        }

        [HttpGet("get-notifications")]
        public async Task<ActionResult<List<Notification>>> GetNotes()
        {
            var userId = GetUserIdFromToken();
            return Ok(await _service.GetNotificationsAsync(userId));
        }

        [HttpPost("create")]
        public async Task<ActionResult<NoteDto>> AddNote([FromBody] CreateNotificationRequest payload)
        {
            var studentId = GetUserIdFromToken();
            return Ok(await _service.CreateNotificationAsync(payload));
        }
    }
}
