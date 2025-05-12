using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SmartTaskAPI.Data;
using SmartTaskAPI.Models;
using SmartTaskAPI.Dtos;
using System.Security.Claims;

namespace SmartTaskAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskItemsController(AppDbContext context)
        {
            _context = context;
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
                .AsQueryable();

            if (!IsAdmin())
                tasksQuery = tasksQuery.Where(t => t.UserId == userId);

            if (query.IsCompleted.HasValue)
                tasksQuery = tasksQuery.Where(t => t.IsCompleted == query.IsCompleted);

            if (!string.IsNullOrWhiteSpace(query.Title))
                tasksQuery = tasksQuery.Where(t => t.Title.Contains(query.Title));

            var totalCount = await tasksQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var tasks = await tasksQuery
                .OrderByDescending(t => t.Id) // İsteğe göre CreatedAt de olabilir
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(t => new TaskItemResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    Username = t.User.Username
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
                Username = task.User.Username
            };
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemResponseDto>> Create(TaskItemCreateDto dto)
        {
            var userId = GetCurrentUserId();

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted,
                UserId = userId
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, new TaskItemResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                Username = User.Identity?.Name!
            });
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskItemUpdateDto dto)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null || (!IsAdmin() && task.UserId != GetCurrentUserId()))
                return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null || (!IsAdmin() && task.UserId != GetCurrentUserId()))
                return NotFound();

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
