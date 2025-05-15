using Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using MyBackedApi.DTOs.Forum.Requests;
using MyBackedApi.DTOs.Forum.Responses;
using MyBackedApi.DTOs.Notifications.Requests;
using MyBackedApi.Enums;
using MyBackedApi.Hubs;
using MyBackedApi.Models;
using MyBackedApi.Services;
using MyBackendApi.Services;

namespace MyBackendApi.Controllers
{
    [Authorize(Roles = "STUDENT, MENTOR, COORDINATOR, ADMIN_CORPORATE")]
    [ApiController]
    [Route("api/forum")]
    public class ForumController : BaseApiController
    {
        private readonly QuestionService _questionService;
        private readonly AnswerService _answerService;
        private readonly NotificationsService _notificationsService;
        private readonly S3Service _s3Service;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ForumController(QuestionService questionService, AnswerService answerService, NotificationsService notificationsService, S3Service s3Service, IHubContext<NotificationHub> hubContext)
        {
            _questionService = questionService;
            _answerService = answerService;
            _notificationsService = notificationsService;
            _s3Service = s3Service;
            _hubContext = hubContext;
        }


        [HttpPost("questions/add")]
        public async Task<IActionResult> AddQuestion([FromForm] AddQuestionRequest payload, [FromForm] List<IFormFile> attachments)
        {
            var currentUserId = GetUserIdFromToken();
            var question = await _questionService.AddQuestionAsync(payload, currentUserId);

            if (attachments != null && attachments.Any())
            {
                var urls = await _s3Service.AddMultipleFilesAsync(attachments, "attachments/" + question.Id.ToString());
            }

            return Ok(new BaseResponseEmpty { Message = "Question added!" });
        }




        [HttpPost("questions")]
        public async Task<GetQuestionsResponse> GetAllQuestions([FromBody] GetQuestionsRequest payload)
        {
            var questions = await _questionService.GetAllQuestionsAsync(payload);
            return questions;
        }


        [HttpGet("questions/{id}")]
        public async Task<GetQuestionResponse> GetQuestionById(Guid id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return null;
            }

            return question;
        }

        [HttpGet("questions/{questionId}/details")]
        public async Task<GetQuestionDetailsResponse> GetQuestionDetails(Guid questionId)
        {
            var answers = await _questionService.GetQuestionDetails(questionId);
            await _s3Service.AddFilesToQuestionDetails(answers);
            return answers;
        }

        [HttpPost("questions/{questionId}/details")]
        public async Task<IActionResult> AddAnswer(Guid questionId, [FromBody] AddAnswerRequest request)
        {
            var currentUserId = GetUserIdFromToken();
            var answer = await _answerService.AddAnswerAsync(questionId, request, currentUserId);
            var questionAuthorId = await _questionService.GetQuestionAuthorByIdAsync(questionId);

            if (ConnectedUsers.UserConnections.TryGetValue(questionAuthorId, out var connectionId))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", new
                {
                    Title = "New answer!",
                    Message = $"{answer.User.Name} {answer.User.Surname} answered your question.",
                    QuestionId = questionId,
                    Type = NotificationTypeEnum.FORUM
                });
                await _notificationsService.CreateNotificationAsync(new CreateNotificationRequest()
                {
                    Title = "New answer!",
                    Message = $"{answer.User.Name} {answer.User.Surname} answered your question.",
                    Type = NotificationTypeEnum.FORUM,
                    Route = "forum/details/" + questionId,
                    UserId = Guid.Parse(questionAuthorId)
                });
            }


            return CreatedAtAction(nameof(AddAnswer), new { questionId }, new {answer.Id, answer.QuestionId, answer.Content, answer.CreatedAt, answer.UserId} );
        }

        [HttpPost("get-related-questions")]
        public async Task<List<GetRelatedQuestionResponse>> GetRelatedQuestions([FromBody] GetRelatedQuestionRequest payload)
        {
            var questions = await _questionService.GetRelatedQuestionsAsync(payload);
            return questions;
        }
    }
}
