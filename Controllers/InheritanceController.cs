using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MirathAI.Api.DTOs;

namespace MirathAI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InheritanceController : ControllerBase
    {
        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] InheritanceRequestDto request)
        {
            // مؤقت: بس عشان نثبت إن التوكن شغال
            return Ok(new { ok = true, madhhab = request.Madhhab });
        }
    }
}
