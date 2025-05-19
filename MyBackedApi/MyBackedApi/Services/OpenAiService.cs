using System.Net.Http.Headers;
using System.Text;
using Infrastructure.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MyBackedApi.Services
{
    public interface IOpenAiService
    {
        Task<float[]> GetEmbeddingAsync(string input);
    }

    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = AppConfig.OpenAISettings.ApiKey;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<float[]> GetEmbeddingAsync(string input)
        {
            var payload = new
            {
                input = input,
                model = "text-embedding-ada-002"
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/embeddings", content);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("OpenAI response failed:");
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content: {result}");
                throw new Exception($"OpenAI API error: {response.StatusCode}");
            }
            response.EnsureSuccessStatusCode();

            dynamic parsed = JsonConvert.DeserializeObject(result);

            var vector = ((IEnumerable<dynamic>)parsed.data[0].embedding).Select(x => (float)x).ToArray();
            return vector;
        }
    }
}
