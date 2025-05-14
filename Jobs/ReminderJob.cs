using SmartTaskAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SmartTaskAPI.Jobs
{
    public class ReminderJob
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReminderJob> _logger;

        public ReminderJob(AppDbContext context, ILogger<ReminderJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SendRemindersAsync()
        {
            var now = DateTime.Now;

            var dueTasks = await _context.TaskItems
                .Where(t => !t.IsDeleted &&
                            !t.IsCompleted &&
                            t.ReminderTime.HasValue &&
                            t.ReminderTime <= now)
                .ToListAsync();

            foreach (var task in dueTasks)
            {
                _logger.LogInformation("🔔 Reminder triggered for task {@TaskId} at {ReminderTime}",
                    task.Id,
                    task.ReminderTime);
            }
        }
    }
}
