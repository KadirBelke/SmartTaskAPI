using Microsoft.Extensions.Logging;

namespace SmartTaskAPI.Services
{
    public class ActivityLogger
    {
        private readonly ILogger<ActivityLogger> _logger;

        public ActivityLogger(ILogger<ActivityLogger> logger)
        {
            _logger = logger;
        }

        public void Log(int userId, string username, string action, int? taskId = null)
        {
            _logger.LogInformation("📘 Activity: {@Activity}",
                new
                {
                    UserId = userId,
                    Username = username,
                    Action = action,
                    TaskId = taskId,
                    Timestamp = DateTime.UtcNow
                });
        }
    }
}
