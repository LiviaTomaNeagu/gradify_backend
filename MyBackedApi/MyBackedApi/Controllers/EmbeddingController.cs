using Microsoft.AspNetCore.Mvc;
using MyBackedApi.Services;
using MyBackendApi.Repositories;
using MyBackendApi.Services;

namespace MyBackedApi.Controllers
{
    [ApiController]
    [Route("api/test-embedding")]
    public class EmbeddingController : ControllerBase
    {
        private readonly IOpenAiService _openAiService;
        private readonly QuestionRepository _questionRepository;

        public EmbeddingController(IOpenAiService openAiService, QuestionRepository questionRepository)
        {
            _openAiService = openAiService;
            _questionRepository = questionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> TestEmbedding([FromBody] EmbeddingTestRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Text is required.");

            var embedding = await _openAiService.GetEmbeddingAsync(request.Text);

            return Ok(new
            {
                Length = embedding.Length,
                Preview = embedding.Take(5) // doar primele 5 valori ca exemplu
            });
        }

        [HttpPost("questions/update-text-from-files")]
        public async Task<IActionResult> PopulateTextFromFiles()
        {
            await _questionRepository.PopulateTextFromFilesForQuestionsAsync();
            return Ok(new { Message = "Done" });
        }

        [HttpPost("questions/update-embeddings")]
        public async Task<IActionResult> GenerateEmbeddings()
        {
            await _questionRepository.GenerateEmbeddingsForAllQuestionsAsync();
            return Ok(new { Message = "Embedding generation complete." });
        }


    }

    public class EmbeddingTestRequest
    {
        public string Text { get; set; }
    }

}
