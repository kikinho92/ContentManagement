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
            modelBuilder.Entity<ContentTag>()
                .HasKey(c => new { c.IdContent, c.IdTag });
        }


        public DbSet<Content> Content { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ContentTag> ContentTag { get; set; }
    }
}