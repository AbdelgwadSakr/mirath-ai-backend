using Microsoft.AspNetCore.Identity;

namespace MirathAI.Api.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
