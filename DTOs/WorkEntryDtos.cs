namespace ConsultantManagementApi.DTOs;

public class WorkEntryDto
{
    public int Id { get; set; }
    public int ConsultantId { get; set; }
    public string ConsultantName { get; set; } = string.Empty;
    public int TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public DateOnly WorkDate { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal RatePerHourAtTimeOfWork { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CreateWorkEntryRequest
{
    public int ConsultantId { get; set; }
    public int TaskId { get; set; }
    public DateOnly WorkDate { get; set; }
    public decimal HoursWorked { get; set; }
}

public class ConsultantPaymentSummaryDto
{
    public int ConsultantId { get; set; }
    public string ConsultantName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TotalHours { get; set; }
    public decimal TotalAmount { get; set; }
    public List<WorkEntryDto> WorkEntries { get; set; } = new List<WorkEntryDto>();
}
