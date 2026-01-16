using System.Text;
using System.Text.Json;

namespace MirathAI.Api.Services
{
    public class GeminiAssistantService
    {
        private readonly HttpClient _http;

        public GeminiAssistantService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> AskAsync(string question)
        {
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new Exception("GEMINI_API_KEY is missing.");

            var prompt =
                "أنت مساعد فقهي متخصص في علم الفرائض (تقسيم الميراث).\n" +
                "أجب بالعربية وبأسلوب واضح ومختصر.\n" +
                "إذا كانت البيانات ناقصة اطلبها أولًا.\n\n" +
                $"سؤال المستخدم: {question}";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            // ✅ موديل مدعوم حسب أمثلة Google API
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            var response = await _http.PostAsync(url, content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(responseText);

            using var doc = JsonDocument.Parse(responseText);

            return doc
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "";
        }
    }
}
