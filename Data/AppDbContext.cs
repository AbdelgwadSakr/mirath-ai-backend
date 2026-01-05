using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MirathAI.Api.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // هنا هتزود DbSet بتاعتك لاحقًا
        // مثال:
        // public DbSet<InheritanceCase> InheritanceCases { get; set; }
    }
}
