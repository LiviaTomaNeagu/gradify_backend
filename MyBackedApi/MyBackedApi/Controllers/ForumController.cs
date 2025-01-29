using Microsoft.AspNetCore.Mvc;
using MyBackedApi.DTOs.Forum.Requests;
using MyBackedApi.DTOs.Forum.Responses;
using MyBackendApi.Services;

namespace MyBackendApi.Controllers
{
    [ApiController]
    [Route("api/forum")]
    public class ForumController : ControllerBase
    {
        private readonly QuestionService _questionService;
        private readonly AnswerService _answerService;

        public ForumController(QuestionService questionService, AnswerService answerService)
        {
            _questionService = questionService;
            _answerService = answerService;
        }


        [HttpPost("questions/add")]
        public async Task<IActionResult> AddQuestion([FromBody] AddQuestionRequest request)
        {
            var question = await _questionService.AddQuestionAsync(request);
            return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, question);
        }


        [HttpPost("questions/{questionId}/answers/add")]
        public async Task<IActionResult> AddAnswer(Guid questionId, [FromBody] AddAnswerRequest request)
        {
            var answer = await _answerService.AddAnswerAsync(questionId, request);
            return CreatedAtAction(nameof(GetAllAnswersForQuestion), new { questionId }, answer);
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

        [HttpGet("questions/{questionId}/answers")]
        public async Task<List<GetAnswerResponse>> GetAllAnswersForQuestion(Guid questionId)
        {
            var answers = await _answerService.GetAllAnswersForQuestionAsync(questionId);
            return answers;
        }

        [HttpGet("questions/{questionId}/details")]
        public async Task<GetQuestionDetailsResponse> GetQuestionDetails(Guid questionId)
        {
            var answers = await _questionService.GetQuestionDetails(questionId);
            return answers;
        }

    }
}
