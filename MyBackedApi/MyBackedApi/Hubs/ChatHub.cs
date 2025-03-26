using Microsoft.AspNetCore.SignalR;
using MyBackedApi.DTOs;
using MyBackedApi.Models;
using MyBackedApi.Services;

namespace MyBackedApi.Hubs
{
    public class ChatHub : Hub
    {
        private ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers.UserConnections[userId] = Context.ConnectionId;
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers.UserConnections.TryRemove(userId, out _);
            }

            return base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(MessagePayload message)
        {
            try
            {
                var senderId = Context.User?.FindFirst("sub")?.Value
                            ?? Context.User?.FindFirst("nameid")?.Value;

                if (string.IsNullOrEmpty(senderId))
                    throw new HubException("User ID is missing in token");

                Console.WriteLine($"Sending message from {senderId} to {message.Id}: {message.Chat[0].Msg}");

                var chatMessage = new ChatMessage
                {
                    Id = Guid.NewGuid(),
                    SenderId = senderId,
                    ReceiverId = message.Id,
                    Message = message.Chat[0].Msg,
                    SentAt = DateTime.UtcNow
                };

                await _chatService.SaveMessage(chatMessage);

                if (ConnectedUsers.UserConnections.TryGetValue(message.Id, out var connectionId))
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR in SendMessage: {ex.Message}");
                throw; // retrimite către client
            }

        }

    }
}
