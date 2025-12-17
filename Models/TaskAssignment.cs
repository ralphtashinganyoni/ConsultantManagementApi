namespace ConsultantManagementApi.Models;

public class TaskAssignment
{
    public int Id { get; set; }
    public int ConsultantId { get; set; }
    public Consultant Consultant { get; set; } = null!;
    public int TaskId { get; set; }
    public ConsultantTask Task { get; set; } = null!;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
