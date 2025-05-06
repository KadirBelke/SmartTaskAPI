namespace SmartTaskAPI.Dtos
{
    public class TaskItemCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
    }

    public class TaskItemUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }

    public class TaskItemResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
