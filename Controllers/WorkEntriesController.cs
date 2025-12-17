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
public class WorkEntriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public WorkEntriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkEntryDto>>> GetWorkEntries()
    {
        var entries = await _context.WorkEntries
            .Include(we => we.Consultant)
            .Include(we => we.Task)
            .Select(we => new WorkEntryDto
            {
                Id = we.Id,
                ConsultantId = we.ConsultantId,
                ConsultantName = $"{we.Consultant.FirstName} {we.Consultant.LastName}",
                TaskId = we.TaskId,
                TaskName = we.Task.Name,
                WorkDate = we.WorkDate,
                HoursWorked = we.HoursWorked,
                RatePerHourAtTimeOfWork = we.RatePerHourAtTimeOfWork,
                TotalAmount = we.HoursWorked * we.RatePerHourAtTimeOfWork
            })
            .ToListAsync();

        return Ok(entries);
    }

    [HttpPost]
    public async Task<ActionResult<WorkEntryDto>> CreateWorkEntry([FromBody] CreateWorkEntryRequest request)
    {
        var consultant = await _context.Consultants
            .Include(c => c.ConsultantRole)
            .FirstOrDefaultAsync(c => c.Id == request.ConsultantId);

        if (consultant == null)
        {
            return BadRequest(new { message = "Consultant not found" });
        }

        var task = await _context.Tasks.FindAsync(request.TaskId);
        if (task == null)
        {
            return BadRequest(new { message = "Task not found" });
        }

        var assignment = await _context.TaskAssignments
            .FirstOrDefaultAsync(ta => ta.ConsultantId == request.ConsultantId && ta.TaskId == request.TaskId);

        if (assignment == null)
        {
            return BadRequest(new { message = "Consultant is not assigned to this task" });
        }

        if (request.HoursWorked <= 0)
        {
            return BadRequest(new { message = "Hours worked must be greater than 0" });
        }

        var totalHoursForDay = await _context.WorkEntries
            .Where(we => we.ConsultantId == request.ConsultantId && we.WorkDate == request.WorkDate)
            .SumAsync(we => we.HoursWorked);

        if (totalHoursForDay + request.HoursWorked > 12)
        {
            return BadRequest(new { message = $"Cannot exceed 12 hours per day. Already worked {totalHoursForDay} hours on this day." });
        }

        var workEntry = new WorkEntry
        {
            ConsultantId = request.ConsultantId,
            TaskId = request.TaskId,
            WorkDate = request.WorkDate,
            HoursWorked = request.HoursWorked,
            RatePerHourAtTimeOfWork = consultant.ConsultantRole.RatePerHour
        };

        _context.WorkEntries.Add(workEntry);
        await _context.SaveChangesAsync();

        var dto = new WorkEntryDto
        {
            Id = workEntry.Id,
            ConsultantId = workEntry.ConsultantId,
            ConsultantName = $"{consultant.FirstName} {consultant.LastName}",
            TaskId = workEntry.TaskId,
            TaskName = task.Name,
            WorkDate = workEntry.WorkDate,
            HoursWorked = workEntry.HoursWorked,
            RatePerHourAtTimeOfWork = workEntry.RatePerHourAtTimeOfWork,
            TotalAmount = workEntry.HoursWorked * workEntry.RatePerHourAtTimeOfWork
        };

        return CreatedAtAction(nameof(GetWorkEntry), new { id = workEntry.Id }, dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkEntryDto>> GetWorkEntry(int id)
    {
        var entry = await _context.WorkEntries
            .Include(we => we.Consultant)
            .Include(we => we.Task)
            .FirstOrDefaultAsync(we => we.Id == id);

        if (entry == null)
        {
            return NotFound();
        }

        return Ok(new WorkEntryDto
        {
            Id = entry.Id,
            ConsultantId = entry.ConsultantId,
            ConsultantName = $"{entry.Consultant.FirstName} {entry.Consultant.LastName}",
            TaskId = entry.TaskId,
            TaskName = entry.Task.Name,
            WorkDate = entry.WorkDate,
            HoursWorked = entry.HoursWorked,
            RatePerHourAtTimeOfWork = entry.RatePerHourAtTimeOfWork,
            TotalAmount = entry.HoursWorked * entry.RatePerHourAtTimeOfWork
        });
    }

    [HttpGet("consultant/{consultantId}/summary")]
    public async Task<ActionResult<ConsultantPaymentSummaryDto>> GetConsultantPaymentSummary(
        int consultantId,
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        var consultant = await _context.Consultants.FindAsync(consultantId);
        if (consultant == null)
        {
            return NotFound(new { message = "Consultant not found" });
        }

        var workEntries = await _context.WorkEntries
            .Include(we => we.Consultant)
            .Include(we => we.Task)
            .Where(we => we.ConsultantId == consultantId && we.WorkDate >= startDate && we.WorkDate <= endDate)
            .OrderBy(we => we.WorkDate)
            .ToListAsync();

        var summary = new ConsultantPaymentSummaryDto
        {
            ConsultantId = consultantId,
            ConsultantName = $"{consultant.FirstName} {consultant.LastName}",
            StartDate = startDate,
            EndDate = endDate,
            TotalHours = workEntries.Sum(we => we.HoursWorked),
            TotalAmount = workEntries.Sum(we => we.HoursWorked * we.RatePerHourAtTimeOfWork),
            WorkEntries = workEntries.Select(we => new WorkEntryDto
            {
                Id = we.Id,
                ConsultantId = we.ConsultantId,
                ConsultantName = $"{we.Consultant.FirstName} {we.Consultant.LastName}",
                TaskId = we.TaskId,
                TaskName = we.Task.Name,
                WorkDate = we.WorkDate,
                HoursWorked = we.HoursWorked,
                RatePerHourAtTimeOfWork = we.RatePerHourAtTimeOfWork,
                TotalAmount = we.HoursWorked * we.RatePerHourAtTimeOfWork
            }).ToList()
        };

        return Ok(summary);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkEntry(int id)
    {
        var entry = await _context.WorkEntries.FindAsync(id);

        if (entry == null)
        {
            return NotFound();
        }

        _context.WorkEntries.Remove(entry);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
