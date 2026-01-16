using Microsoft.AspNetCore.Mvc;
using MirathAI.Api.DTOs;
using MirathAI.Api.Services;
using MirathAI.Api.Enums;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MirathAI.Api.Controllers
{
    [ApiController]
    [Route("api/ollama")]
    public class OllamaController : ControllerBase
    {
        private readonly OllamaService _ollama;
        private readonly InheritanceCalculatorDispatcher _dispatcher;

        public OllamaController(
            OllamaService ollama,
            InheritanceCalculatorDispatcher dispatcher)
        {
            _ollama = ollama;
            _dispatcher = dispatcher;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AssistantRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("السؤال مطلوب");

            /* ================= 1️⃣ Ollama (أرقام فقط) ================= */

            var prompt = """
أنت برنامج استخراج أرقام فقط.
لا تفهم.
لا تحلل.
لا تضف أي معلومة.

أخرج JSON فقط بالشكل التالي EXACTLY:

{
  "deceasedGender": "male|female",
  "wivesCount": number,
  "estate": number
}

قواعد:
- غير المذكور = 0
- لا تضف حقول
- لا تكتب أي نص خارج JSON

النص:
""" + request.Question;

            string raw;
            try
            {
                raw = await _ollama.AskAsync(prompt);
            }
            catch
            {
                return StatusCode(500, "فشل الاتصال بـ Ollama");
            }

            var match = Regex.Match(raw, @"\{[\s\S]*\}");
            if (!match.Success)
                return BadRequest("Ollama لم يرجع JSON صالح");

            JsonElement parsed;
            try
            {
                parsed = JsonSerializer.Deserialize<JsonElement>(match.Value);
            }
            catch
            {
                return BadRequest("JSON غير صالح");
            }

            /* ================= 2️⃣ التحليل العربي الحقيقي ================= */

            string q = request.Question;

            bool hasHusband = Regex.IsMatch(q, @"\bزوج\b|\bزوجها\b");
            bool hasWife = Regex.IsMatch(q, @"\bزوجة\b|\bزوجته\b");

            bool hasMother = Regex.IsMatch(q, @"\b(أم|ام)\b");
            bool hasFather = Regex.IsMatch(q, @"\b(أب|اب)\b");

            int sons = Regex.Matches(q, @"\b(ابن|ابنين|أبناء)\b").Count;
            int daughters = Regex.Matches(q, @"\b(بنت|بنتين|بنات)\b").Count;

            int brothers = Regex.Matches(q, @"\b(أخ|اخ|أخوه|وأخ)\b").Count;
            int sisters = Regex.Matches(q, @"\b(أخت|اخت|أختها|وأخت)\b").Count;

            /* ================= 3️⃣ DTO نهائي مضمون ================= */

            var dto = new InheritanceRequestDto
            {
                DeceasedGender = GetString(parsed, "deceasedGender", "male"),

                HasSpouse = hasHusband || hasWife,
                WivesCount = hasWife ? GetInt(parsed, "wivesCount") : 0,

                Sons = sons,
                Daughters = daughters,

                HasFather = hasFather,
                HasMother = hasMother,

                Brothers = brothers,
                Sisters = sisters,

                HasCash = true,
                CashAmount = GetDecimal(parsed, "estate")
            };

            // 🔒 قاعدة فقهية حاسمة
            if (dto.DeceasedGender == "female")
            {
                dto.Sons = 0;
                dto.Daughters = 0;
            }

            /* ================= 4️⃣ الحساب لكل المذاهب ================= */

            var results = new Dictionary<string, InheritanceResultDto>
            {
                ["jumhur"] = _dispatcher.Calculate(dto, FiqhMadhhab.Jumhur),
                ["hanafi"] = _dispatcher.Calculate(dto, FiqhMadhhab.Hanafi),
                ["maliki"] = _dispatcher.Calculate(dto, FiqhMadhhab.Maliki),
                ["shafii"] = _dispatcher.Calculate(dto, FiqhMadhhab.Shafii),
                ["hanbali"] = _dispatcher.Calculate(dto, FiqhMadhhab.Hanbali),
            };

            return Ok(new
            {
                answer = "تم حساب الميراث بنجاح ✅",
                input = dto,
                results
            });
        }

        /* ================= Helpers ================= */

        private static int GetInt(JsonElement e, string name)
            => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number
                ? v.GetInt32()
                : 0;

        private static string GetString(JsonElement e, string name, string def)
            => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String
                ? v.GetString() ?? def
                : def;

        private static decimal GetDecimal(JsonElement e, string name)
            => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number
                ? v.GetDecimal()
                : 0m;
    }
}
