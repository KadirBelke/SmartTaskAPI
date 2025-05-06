namespace SmartTaskAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; } // identity (auto-increment)
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
