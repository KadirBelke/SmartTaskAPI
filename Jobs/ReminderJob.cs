using SmartTaskAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartTaskAPI.Jobs
{
    public class ReminderJob
    {
        private readonly AppDbContext _context;

        public ReminderJob(AppDbContext context)
        {
            _context = context;
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
                // At the moment we are only writing logs, e-mail, socket etc. can be added in the future
                Console.WriteLine($"🔔 Reminder: \"{task.Title}\" is due! [#{task.Id}] at {task.ReminderTime}");
            }
        }
    }
}
