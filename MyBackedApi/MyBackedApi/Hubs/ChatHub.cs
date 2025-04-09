using Microsoft.AspNetCore.SignalR;
using MyBackedApi.DTOs;
using MyBackedApi.Models;
using MyBackedApi.Services;
using static MyBackedApi.DTOs.MessagePayload;

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
            try
            {
                var httpContext = Context.GetHttpContext();
                var userId = httpContext?.Request.Query["userId"].ToString();
                if (!string.IsNullOrEmpty(userId))
                {
                    ConnectedUsers.UserConnections[userId] = Context.ConnectionId;
                }
                Console.WriteLine($"Client conectat: {Context.ConnectionId}");

                return base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare in OnConnected");
                return base.OnConnectedAsync();
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Request.Query["userId"].ToString();
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
                var httpContext = Context.GetHttpContext();
                var senderId = httpContext?.Request.Query["userId"].ToString();

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
                throw;
            }

        }

    }
}
