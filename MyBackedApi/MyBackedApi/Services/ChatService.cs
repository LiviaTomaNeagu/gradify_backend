using Google.Protobuf;
using MyBackedApi.Data;
using MyBackedApi.DTOs.Chat.Responses;
using MyBackedApi.Models;
using MyBackedApi.Repositories;
using MyBackendApi.Repositories;
using System;

namespace MyBackedApi.Services
{
    public class ChatService
    {
        private readonly ChatRepository _chatRepository;
        private readonly UserRepository _userRepository;

        public ChatService(
            ChatRepository chatRepository,
            UserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task SaveMessage(ChatMessage message)
        {
            await _chatRepository.AddAsync(message);
        }

        public async Task<List<ChatMessage>> GetMessagesBetween(string userA, string userB)
        {
            return await _chatRepository.GetMessagesBetweenAsync(userA, userB);
        }

        public async Task<GetUserConversationsResponse> GetAllMessagesForUserAsync(Guid userId)
        {
            var chatMessages = await _chatRepository.GetAllMessagesForUserAsync(userId);

            var grouped = chatMessages
                    .GroupBy(m => m.SenderId == userId.ToString() ? m.ReceiverId : m.SenderId);

            var conversations = new List<GetConversation>();

            foreach (var g in grouped)
            {
                var messages = g.OrderBy(m => m.SentAt).ToList();
                var interactedUserId = g.Key;

                var interactedUser = await _userRepository.GetUserByIdAsync(Guid.Parse(interactedUserId));

                conversations.Add(new GetConversation
                {
                    Id = interactedUserId,
                    From = $"{interactedUser.Name} {interactedUser.Surname}",
                    Subject = "Conversation",
                    Chat = messages.Select(m => new ChatEntryDto
                    {
                        Type = m.SenderId == userId.ToString() ? "odd" : "even",
                        Msg = m.Message,
                        Date = m.SentAt
                    }).ToList()
                });
            }

            return new GetUserConversationsResponse
            {
                Conversations = conversations
            };

        }

    }
}
