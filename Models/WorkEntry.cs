namespace ConsultantManagementApi.Models;

public class WorkEntry
{
    public int Id { get; set; }
    public int ConsultantId { get; set; }
    public Consultant Consultant { get; set; } = null!;
    public int TaskId { get; set; }
    public ConsultantTask Task { get; set; } = null!;
    public DateOnly WorkDate { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal RatePerHourAtTimeOfWork { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
