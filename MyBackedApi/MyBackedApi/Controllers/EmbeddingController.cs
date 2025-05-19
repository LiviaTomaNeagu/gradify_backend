using Microsoft.AspNetCore.Mvc;
using MyBackedApi.Services;

namespace MyBackedApi.Controllers
{
    [ApiController]
    [Route("api/test-embedding")]
    public class EmbeddingController : ControllerBase
    {
        private readonly IOpenAiService _openAiService;

        public EmbeddingController(IOpenAiService openAiService)
        {
            _openAiService = openAiService;
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
    }

    public class EmbeddingTestRequest
    {
        public string Text { get; set; }
    }

}
