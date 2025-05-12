using System.ComponentModel.DataAnnotations;

namespace SmartTaskAPI.Models
{
    public class User
    {
        public enum RoleType { User, Admin }
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public RoleType Role { get; set; } = RoleType.User;
        public List<TaskItem> TaskItems { get; set; } = new();

    }
}
