using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MirathAI.Api.Services
{
    public class OpenAiAssistantService
    {
        private readonly HttpClient _http;

        public OpenAiAssistantService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> AskAsync(string question)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new Exception("OPENAI_API_KEY is missing.");

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var prompt =
                "أنت مساعد فقهي متخصص في علم الفرائض (تقسيم الميراث).\n" +
                "أجب باللغة العربية وبأسلوب واضح.\n" +
                "إذا كانت البيانات ناقصة، اطلب البيانات الناقصة أولًا.\n\n" +
                $"سؤال المستخدم: {question}";

            var payload = new
            {
                model = "gpt-4.1-mini",
                input = prompt
            };

            var json = JsonSerializer.Serialize(payload);
            using var content =
                new StringContent(json, Encoding.UTF8, "application/json");

            using var response =
                await _http.PostAsync("https://api.openai.com/v1/responses", content);

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(responseText);

            using var doc = JsonDocument.Parse(responseText);

            if (doc.RootElement.TryGetProperty("output_text", out var outputText))
                return outputText.GetString() ?? "";

            return responseText;
        }
    }
}
