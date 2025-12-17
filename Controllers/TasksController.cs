using ConsultantManagementApi.Data;
using ConsultantManagementApi.DTOs;
using ConsultantManagementApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultantManagementApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
    {
        var tasks = await _context.Tasks
            .Include(t => t.TaskAssignments)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                DurationHours = t.DurationHours,
                AssignedConsultantIds = t.TaskAssignments.Select(ta => ta.ConsultantId).ToList()
            })
            .ToListAsync();

        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTask(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.TaskAssignments)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        return Ok(new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            DurationHours = task.DurationHours,
            AssignedConsultantIds = task.TaskAssignments.Select(ta => ta.ConsultantId).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskRequest request)
    {
        var task = new ConsultantTask
        {
            Name = request.Name,
            Description = request.Description,
            DurationHours = request.DurationHours
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var dto = new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            DurationHours = task.DurationHours,
            AssignedConsultantIds = new List<int>()
        };

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, dto);
    }

    [HttpPost("{taskId}/assign")]
    public async Task<IActionResult> AssignConsultant(int taskId, [FromBody] AssignConsultantToTaskRequest request)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
        {
            return NotFound(new { message = "Task not found" });
        }

        var consultant = await _context.Consultants.FindAsync(request.ConsultantId);
        if (consultant == null)
        {
            return BadRequest(new { message = "Consultant not found" });
        }

        var existingAssignment = await _context.TaskAssignments
            .FirstOrDefaultAsync(ta => ta.TaskId == taskId && ta.ConsultantId == request.ConsultantId);

        if (existingAssignment != null)
        {
            return BadRequest(new { message = "Consultant already assigned to this task" });
        }

        var assignment = new TaskAssignment
        {
            TaskId = taskId,
            ConsultantId = request.ConsultantId
        };

        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Consultant assigned successfully" });
    }

    [HttpDelete("{taskId}/unassign/{consultantId}")]
    public async Task<IActionResult> UnassignConsultant(int taskId, int consultantId)
    {
        var assignment = await _context.TaskAssignments
            .FirstOrDefaultAsync(ta => ta.TaskId == taskId && ta.ConsultantId == consultantId);

        if (assignment == null)
        {
            return NotFound(new { message = "Assignment not found" });
        }

        _context.TaskAssignments.Remove(assignment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
