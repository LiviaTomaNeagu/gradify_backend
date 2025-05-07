using Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.Chat.Responses;
using MyBackedApi.Services;

namespace MyBackedApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/chat")]
    public class ChatController : BaseApiController
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("get-messages")]
        public async Task<ActionResult<GetUserConversationsResponse>> GetUserConversations()
        {
            var currentUserId = GetUserIdFromToken();
            var messages = await _chatService.GetAllMessagesForUserAsync(currentUserId);
            return Ok(messages);
        }

    }
}
