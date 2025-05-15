using Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs;
using MyBackedApi.DTOs.Notifications.Requests;
using MyBackedApi.DTOs.Notifications.Responses;
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

        [HttpGet("unread-count")]
        public async Task<ActionResult<UnreadCountResponse>> UnreadCount()
        {
            var userId = GetUserIdFromToken();
            return await _service.UnreadCountAsync(userId);
        }

        [HttpPost("create")]
        public async Task<ActionResult<NoteDto>> AddNote([FromBody] CreateNotificationRequest payload)
        {
            var studentId = GetUserIdFromToken();
            return Ok(await _service.CreateNotificationAsync(payload));
        }

        [HttpPut("read/{notificationId}")]
        public async Task<IActionResult> ReadNotification([FromRoute] Guid notificationId)
        {
            await _service.ReadNotificationAsync(notificationId);
            return Ok(new BaseResponseEmpty() 
            { Message = "Notification created!" });
        }
    }
}
