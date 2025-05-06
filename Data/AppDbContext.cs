using Microsoft.EntityFrameworkCore;
using SmartTaskAPI.Models;

namespace SmartTaskAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TaskItem> TaskItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
