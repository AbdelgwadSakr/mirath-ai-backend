using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MirathAI.Api.Services;

namespace MirathAI.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtTokenService _jwt;

        public AuthController(UserManager<IdentityUser> userManager, JwtTokenService jwt)
        {
            _userManager = userManager;
            _jwt = jwt;
        }

        public record RegisterDto(string FullName, string Email, string Password);
        public record LoginDto(string Email, string Password);

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return BadRequest(new { message = "Email already exists" });

            var user = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description).ToList() });

            var token = _jwt.CreateToken(user);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            var ok = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!ok)
                return Unauthorized(new { message = "Invalid credentials" });

            var token = _jwt.CreateToken(user);
            return Ok(new { token });
        }
    }
}
