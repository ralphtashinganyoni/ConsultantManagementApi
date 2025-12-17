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
public class ConsultantsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ConsultantsController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConsultantDto>>> GetConsultants()
    {
        var consultants = await _context.Consultants
            .Include(c => c.ConsultantRole)
            .Select(c => new ConsultantDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                ProfileImagePath = c.ProfileImagePath,
                ConsultantRoleId = c.ConsultantRoleId,
                ConsultantRoleName = c.ConsultantRole.Name,
                CurrentRatePerHour = c.ConsultantRole.RatePerHour
            })
            .ToListAsync();

        return Ok(consultants);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ConsultantDto>> GetConsultant(int id)
    {
        var consultant = await _context.Consultants
            .Include(c => c.ConsultantRole)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (consultant == null)
        {
            return NotFound();
        }

        return Ok(new ConsultantDto
        {
            Id = consultant.Id,
            FirstName = consultant.FirstName,
            LastName = consultant.LastName,
            Email = consultant.Email,
            ProfileImagePath = consultant.ProfileImagePath,
            ConsultantRoleId = consultant.ConsultantRoleId,
            ConsultantRoleName = consultant.ConsultantRole.Name,
            CurrentRatePerHour = consultant.ConsultantRole.RatePerHour
        });
    }

    [HttpPost]
    public async Task<ActionResult<ConsultantDto>> CreateConsultant([FromBody] CreateConsultantRequest request)
    {
        var role = await _context.ConsultantRoles.FindAsync(request.ConsultantRoleId);
        if (role == null)
        {
            return BadRequest(new { message = "Invalid role ID" });
        }

        var consultant = new Consultant
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            ConsultantRoleId = request.ConsultantRoleId
        };

        _context.Consultants.Add(consultant);
        await _context.SaveChangesAsync();

        var dto = new ConsultantDto
        {
            Id = consultant.Id,
            FirstName = consultant.FirstName,
            LastName = consultant.LastName,
            Email = consultant.Email,
            ProfileImagePath = consultant.ProfileImagePath,
            ConsultantRoleId = consultant.ConsultantRoleId,
            ConsultantRoleName = role.Name,
            CurrentRatePerHour = role.RatePerHour
        };

        return CreatedAtAction(nameof(GetConsultant), new { id = consultant.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateConsultant(int id, [FromBody] UpdateConsultantRequest request)
    {
        var consultant = await _context.Consultants.FindAsync(id);

        if (consultant == null)
        {
            return NotFound();
        }

        var role = await _context.ConsultantRoles.FindAsync(request.ConsultantRoleId);
        if (role == null)
        {
            return BadRequest(new { message = "Invalid role ID" });
        }

        consultant.FirstName = request.FirstName;
        consultant.LastName = request.LastName;
        consultant.Email = request.Email;
        consultant.ConsultantRoleId = request.ConsultantRoleId;
        consultant.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/profile-image")]
    public async Task<IActionResult> UploadProfileImage(int id, IFormFile file)
    {
        var consultant = await _context.Consultants.FindAsync(id);

        if (consultant == null)
        {
            return NotFound();
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file provided" });
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(new { message = "Invalid file type. Only images are allowed." });
        }

        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads", "profiles");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{consultant.Id}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        consultant.ProfileImagePath = $"/uploads/profiles/{fileName}";
        consultant.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { imagePath = consultant.ProfileImagePath });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConsultant(int id)
    {
        var consultant = await _context.Consultants.FindAsync(id);

        if (consultant == null)
        {
            return NotFound();
        }

        _context.Consultants.Remove(consultant);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
