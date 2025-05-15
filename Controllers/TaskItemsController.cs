using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SmartTaskAPI.Data;
using SmartTaskAPI.Models;
using SmartTaskAPI.Dtos;
using SmartTaskAPI.Services;
using System.Security.Claims;

namespace SmartTaskAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ActivityLogger _activityLogger;

        public TaskItemsController(AppDbContext context, ActivityLogger activityLogger)
        {
            _context = context;
            _activityLogger = activityLogger;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<TaskItemResponseDto>>> GetAll([FromQuery] TaskItemQueryDto query)
        {
            var userId = GetCurrentUserId();

            var tasksQuery = _context.TaskItems
                .Include(t => t.User)
                .Include(t => t.TaskItemTags)
                    .ThenInclude(tt => tt.Tag)
                .AsQueryable();

            if (!IsAdmin())
                tasksQuery = tasksQuery.Where(t => t.UserId == userId);

            if (query.IsCompleted.HasValue)
                tasksQuery = tasksQuery.Where(t => t.IsCompleted == query.IsCompleted);

            if (!string.IsNullOrWhiteSpace(query.Title))
                tasksQuery = tasksQuery.Where(t => t.Title.Contains(query.Title));

            if (!string.IsNullOrWhiteSpace(query.Tag))
                tasksQuery = tasksQuery.Where(t => t.TaskItemTags.Any(tt => tt.Tag.Name == query.Tag));

            if (query.ReminderDue == true)
            {
                var now = DateTime.Now;
                tasksQuery = tasksQuery
                    .Where(t => t.ReminderTime.HasValue && t.ReminderTime <= now);
            }

            var totalCount = await tasksQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var tasks = await tasksQuery
                .OrderByDescending(t => t.Id)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(t => new TaskItemResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    Username = t.User.Username,
                    Tags = t.TaskItemTags.Select(tt => tt.Tag.Name).ToList(),
                    DueDate = t.DueDate,
                    ReminderTime = t.ReminderTime
                })
                .ToListAsync();

            var response = new PaginatedResponse<TaskItemResponseDto>
            {
                CurrentPage = query.Page,
                TotalPages = totalPages,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = tasks
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemResponseDto>> GetById(int id)
        {
            var task = await _context.TaskItems
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null || (!IsAdmin() && task.UserId != GetCurrentUserId()))
                return NotFound();

            return new TaskItemResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                Username = task.User.Username,
                Tags = task.TaskItemTags.Select(tt => tt.Tag.Name).ToList(),
                DueDate = task.DueDate,
                ReminderTime = task.ReminderTime
            };
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemResponseDto>> Create(TaskItemCreateDto dto)
        {
            var userId = GetCurrentUserId();

            var taskItem = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted,
                UserId = userId,
                DueDate = dto.DueDate,
                ReminderTime = dto.ReminderTime
            };

            if (dto.Tags != null)
            {
                foreach (var tagName in dto.Tags.Distinct())
                {
                    var tag = await _context.Tags
                            .FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower())
                            ?? new Tag { Name = tagName };

                    taskItem.TaskItemTags.Add(new TaskItemTag { Tag = tag });
                }
            }

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            _activityLogger.Log(userId, User.Identity?.Name ?? "unknown", "Created Task", taskItem.Id);

            var response = new TaskItemResponseDto
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                IsCompleted = taskItem.IsCompleted,
                Username = User.Identity?.Name ?? "unknown",
                Tags = taskItem.TaskItemTags.Select(tt => tt.Tag.Name).ToList(),
                DueDate = taskItem.DueDate,
                ReminderTime = taskItem.ReminderTime
            };

            return CreatedAtAction(nameof(GetById), new { id = taskItem.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, TaskItemUpdateDto dto)
        {
            var task = await _context.TaskItems
                .Include(t => t.TaskItemTags)
                .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound();

            if (!IsAdmin() && task.UserId != GetCurrentUserId())
                return Forbid();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;
            task.DueDate = dto.DueDate;
            task.ReminderTime = dto.ReminderTime;
            task.TaskItemTags.Clear();

            if (dto.Tags != null)
            {
                foreach (var tagName in dto.Tags.Distinct())
                {
                    var tag = await _context.Tags
                        .FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower())
                        ?? new Tag { Name = tagName };

                    task.TaskItemTags.Add(new TaskItemTag { Tag = tag });
                }
            }

            await _context.SaveChangesAsync();

            _activityLogger.Log(GetCurrentUserId(), User.Identity?.Name ?? "unknown", "Updated Task", task.Id);

            var response = new TaskItemResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                Username = User.Identity?.Name ?? "unknown",
                Tags = task.TaskItemTags.Select(tt => tt.Tag.Name).ToList()
            };

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null || (!IsAdmin() && task.UserId != GetCurrentUserId()))
                return NotFound();

            task.IsDeleted = true;
            await _context.SaveChangesAsync();

            _activityLogger.Log(GetCurrentUserId(), User.Identity?.Name ?? "unknown", "Deleted Task", task.Id);

            return NoContent();
        }

        [HttpGet("tags")]
        public async Task<ActionResult<List<string>>> GetAllTags()
        {
            var userId = GetCurrentUserId();

            var tagsQuery = _context.Tags
                .Include(t => t.TaskItemTags)
                .ThenInclude(tt => tt.TaskItem)
                .AsQueryable();

            if (!IsAdmin())
            {
                tagsQuery = tagsQuery
                    .Where(t => t.TaskItemTags
                        .Any(tt => tt.TaskItem.UserId == userId));
            }

            var tags = await tagsQuery
                .Select(t => t.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();

            return Ok(tags);
        }

        [HttpGet("tags/stats")]
        public async Task<ActionResult> GetTagStats()
        {
            var userId = GetCurrentUserId();

            var tagsQuery = _context.Tags
                .Include(t => t.TaskItemTags)
                    .ThenInclude(tt => tt.TaskItem)
                .AsQueryable();

            if (!IsAdmin())
            {
                tagsQuery = tagsQuery
                    .Where(t => t.TaskItemTags
                        .Any(tt => tt.TaskItem.UserId == userId));
            }

            var stats = await tagsQuery
                .Select(tag => new
                {
                    Tag = tag.Name,
                    Count = tag.TaskItemTags
                        .Count(tt => tt.TaskItem.UserId == userId || IsAdmin())
                })
                .OrderByDescending(t => t.Count)
                .ToListAsync();

            return Ok(stats);
        }
    }
}