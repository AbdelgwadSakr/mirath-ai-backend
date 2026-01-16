using Microsoft.AspNetCore.Mvc;
using MirathAI.Api.DTOs;
using MirathAI.Api.Services;

namespace MirathAI.Api.Controllers
{
    [ApiController]
    [Route("api/assistant")]
    public class AssistantController : ControllerBase
    {
        private readonly OllamaService _ollama;

        public AssistantController(OllamaService ollama)
        {
            _ollama = ollama;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AssistantRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question is required");

            var prompt = $@"
أنت فقيه متخصص في علم المواريث الإسلامية.
أجب عن السؤال التالي إجابة واضحة ومنظمة وبالعربي:

{request.Question}
";

            var answer = await _ollama.AskAsync(prompt);

            return Ok(new
            {
                Answer = answer
            });
        }
    }

    
}
