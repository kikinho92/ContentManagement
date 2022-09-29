using Microsoft.EntityFrameworkCore;

namespace Service.Content.Data
{
    public class ContentDbContext : DbContext
    {
        public ContentDbContext (DbContextOptions<ContentDbContext> options)
            : base(options)
        {
        }
		
		
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContentTags>()
                .HasKey(c => new { c.IdContent, c.IdTag });
        }


        public DbSet<Contents> Contents { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<ContentTags> ContentTags { get; set; }
    }
}