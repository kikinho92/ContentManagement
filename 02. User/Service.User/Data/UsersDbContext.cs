using Microsoft.EntityFrameworkCore;

namespace Service.User.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext (DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Groups> Groups { get; set; }
    }
}