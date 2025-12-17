namespace ConsultantManagementApi.DTOs;

public class TaskDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DurationHours { get; set; }
    public List<int> AssignedConsultantIds { get; set; } = new List<int>();
}

public class CreateTaskRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DurationHours { get; set; }
}

public class AssignConsultantToTaskRequest
{
    public int ConsultantId { get; set; }
}
