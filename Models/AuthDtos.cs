namespace MirathAI.Api.Models
{
    public record RegisterRequest(string FullName, string Email, string Password);
    public record LoginRequest(string Email, string Password);

    public record AuthResponse(
        string Token,
        DateTime ExpiresAt,
        string UserId,
        string Email
    );
}
