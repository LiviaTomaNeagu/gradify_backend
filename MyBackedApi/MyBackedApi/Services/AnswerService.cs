﻿using MyBackedApi.DTOs.Forum.Requests;
using MyBackedApi.DTOs.Forum.Responses;
using MyBackedApi.Models;
using MyBackendApi.Mappings;
using MyBackendApi.Repositories;

namespace MyBackendApi.Services
{
    public class AnswerService
    {
        private readonly AnswerRepository _answerRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly UserRepository _userRepository;

        public AnswerService(
            AnswerRepository answerRepository, 
            QuestionRepository questionRepository,
            UserRepository userRepository)
        {
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
            _userRepository = userRepository;
        }

        public async Task<Answer> AddAnswerAsync(Guid questionId, AddAnswerRequest request, Guid currentUserId)
        {
            var question = await _questionRepository.GetQuestionByIdAsync(questionId);
            if (question == null)
            {
                throw new KeyNotFoundException("Question not found");
            }

            var user = await _userRepository.GetUserByIdAsync(currentUserId);
            if(user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var answer = new Answer
            {
                Id = Guid.NewGuid(),
                QuestionId = questionId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UserId = currentUserId,
                User = user
            };

            await _answerRepository.AddAnswerAsync(answer);
            return answer;
        }

    }
}
