using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MirathAI.Api.Services
{
    public class OllamaService
    {
        private readonly HttpClient _http;

        public OllamaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> AskAsync(string prompt)
        {
            var body = new
            {
                model = "mistral",
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _http.PostAsync(
                "http://localhost:11434/api/generate",
                content
            );

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement.GetProperty("response").GetString()!;
        }
    }
}
