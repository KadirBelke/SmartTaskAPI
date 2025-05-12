using Microsoft.EntityFrameworkCore;
using SmartTaskAPI.Models;

namespace SmartTaskAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<TaskItemTag> TaskItemTags { get; set; } = null!;


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<TaskItemTag>()
                .HasKey(tt => new { tt.TaskItemId, tt.TagId });

            modelBuilder.Entity<TaskItemTag>()
                .HasOne(tt => tt.TaskItem)
                .WithMany(t => t.TaskItemTags)
                .HasForeignKey(tt => tt.TaskItemId);

            modelBuilder.Entity<TaskItemTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskItemTags)
                .HasForeignKey(tt => tt.TagId);
        }
    }
}
