using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Service.Auth.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext (DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
    }
}