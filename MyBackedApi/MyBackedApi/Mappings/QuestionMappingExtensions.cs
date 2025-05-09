﻿using MyBackedApi.DTOs.Forum;
using MyBackedApi.DTOs.Forum.Responses;
using MyBackedApi.DTOs.User;
using MyBackedApi.Enums;
using MyBackedApi.Models;
using MyBackendApi.Mappings;

namespace MyBackedApi.Mappings
{
    public static class QuestionMappingExtensions
    {
        public static GetQuestionResponse ToGetQuestionResponse(this Question question)
        {
            return new GetQuestionResponse
            {
                Id = question.Id,
                Title = question.Title,
                Content = question.Content,
                CreatedAt = question.CreatedAt,
                UserId = question.UserId,
                Topic = question.Topic,
                Name = question.User.Name,
                Surname = question.User.Surname,
                OccupationName = question.User.Occupation.Name
            };
        }

        public static List<GetQuestionResponse> ToGetQuestionResponses(this IEnumerable<Question> questions)
        {
            return questions.Select(q => q.ToGetQuestionResponse()).ToList();
        }

        public static GetQuestionDetailsResponse ToGetQuestionDetailsResponse(this Question question)
        {
            return new GetQuestionDetailsResponse
            {
                Id = question.Id,
                Title = question.Title,
                Content = question.Content,
                CreatedAt = question.CreatedAt,
                UserId = question.UserId,
                Topic = question.Topic,
                Name = question.User.Name,
                Surname = question.User.Surname,
                OccupationName = question.User.Occupation.Name,
                Answers = question.Answers.ToGetAnswerResponses()
            };
        }

        public static GetRelatedQuestionResponse ToRelatedQuestionResponse(this Question question)
        {
            return new GetRelatedQuestionResponse
            {
                Id = question.Id,
                Title = question.Title,
                AnswersCount = question.Answers?.Count ?? 0,
                Topic = question.Topic
            };
        }

        public static LatestQuestion ToLatestQuestion(this Question question)
        {
            return new LatestQuestion
            {
                Id = question.Id,
                Title = question.Title,
                CreatedAt = question.CreatedAt,
                Topic = question.Topic,
                Author = question.User.ToShortUserDto()
            };
        }

        public static List<LatestQuestion> ToLatestQuestions(this List<Question> questions)
        {
            return questions.Select(q => q.ToLatestQuestion()).ToList();
        }
    }
}