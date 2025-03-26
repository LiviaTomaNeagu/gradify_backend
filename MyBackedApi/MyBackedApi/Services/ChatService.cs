using MyBackedApi.Data;
using MyBackedApi.Models;
using MyBackedApi.Repositories;
using System;

namespace MyBackedApi.Services
{
    public class ChatService
    {
        private readonly ChatRepository _chatRepository;

        public ChatService(ChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task SaveMessage(ChatMessage message)
        {
            await _chatRepository.AddAsync(message);
        }

        public async Task<List<ChatMessage>> GetMessagesBetween(string userA, string userB)
        {
            return await _chatRepository.GetMessagesBetweenAsync(userA, userB);
        }
    }
}
