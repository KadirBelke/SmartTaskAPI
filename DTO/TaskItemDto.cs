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

    public class TaskItemQueryDto
    {
        public bool? IsCompleted { get; set; }
        public string? Title { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
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
