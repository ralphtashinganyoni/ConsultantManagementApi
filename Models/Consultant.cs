namespace ConsultantManagementApi.Models;

public class Consultant
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }
    public int ConsultantRoleId { get; set; }
    public ConsultantRole ConsultantRole { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
    public ICollection<WorkEntry> WorkEntries { get; set; } = new List<WorkEntry>();
}
