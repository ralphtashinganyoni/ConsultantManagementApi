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
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RolesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConsultantRoleDto>>> GetRoles()
    {
        var roles = await _context.ConsultantRoles
            .Select(r => new ConsultantRoleDto
            {
                Id = r.Id,
                Name = r.Name,
                RatePerHour = r.RatePerHour
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ConsultantRoleDto>> GetRole(int id)
    {
        var role = await _context.ConsultantRoles.FindAsync(id);

        if (role == null)
        {
            return NotFound();
        }

        return Ok(new ConsultantRoleDto
        {
            Id = role.Id,
            Name = role.Name,
            RatePerHour = role.RatePerHour
        });
    }

    [HttpPost]
    public async Task<ActionResult<ConsultantRoleDto>> CreateRole([FromBody] CreateRoleRequest request)
    {
        var role = new ConsultantRole
        {
            Name = request.Name,
            RatePerHour = request.RatePerHour
        };

        _context.ConsultantRoles.Add(role);
        await _context.SaveChangesAsync();

        var dto = new ConsultantRoleDto
        {
            Id = role.Id,
            Name = role.Name,
            RatePerHour = role.RatePerHour
        };

        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
    {
        var role = await _context.ConsultantRoles.FindAsync(id);

        if (role == null)
        {
            return NotFound();
        }

        role.Name = request.Name;
        role.RatePerHour = request.RatePerHour;
        role.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _context.ConsultantRoles.FindAsync(id);

        if (role == null)
        {
            return NotFound();
        }

        _context.ConsultantRoles.Remove(role);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
