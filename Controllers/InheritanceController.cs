using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MirathAI.Api.DTOs;
using MirathAI.Api.Services;

namespace MirathAI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InheritanceController : ControllerBase
    {
        private readonly InheritanceCalculationService _service;

        public InheritanceController(InheritanceCalculationService service)
        {
            _service = service;
        }

        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] InheritanceRequestDto request)
        {
            var result = _service.Calculate(request);
            return Ok(result);
        }
    }
}
