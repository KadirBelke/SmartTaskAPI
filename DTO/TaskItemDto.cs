namespace SmartTaskAPI.Dtos
{
    public class TaskItemCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public List<string>? Tags { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReminderTime { get; set; }
    }

    public class TaskItemUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public List<string>? Tags { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReminderTime { get; set; }
    }

    public class TaskItemResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public DateTime? DueDate { get; set; }
        public DateTime? ReminderTime { get; set; }
    }

    public class TaskItemQueryDto
    {
        public bool? IsCompleted { get; set; }
        public string? Title { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Tag { get; set; }
        public bool? ReminderDue { get; set; }
    }

    public class PaginatedResponse<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
