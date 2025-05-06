using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SmartTaskAPI.Data;
using SmartTaskAPI.Models;
using SmartTaskAPI.Dtos;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetAll()
        {
            var tasks = await _context.TaskItems.ToListAsync();
            var result = tasks.Select(t => new TaskItemResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemResponseDto>> GetById(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();

            var result = new TaskItemResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemResponseDto>> Create(TaskItemCreateDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            var response = new TaskItemResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted
            };

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskItemUpdateDto dto)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null) return NotFound();

            taskItem.Title = dto.Title;
            taskItem.Description = dto.Description;
            taskItem.IsCompleted = dto.IsCompleted;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
