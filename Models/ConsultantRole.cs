namespace ConsultantManagementApi.Models;

public class ConsultantRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal RatePerHour { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Consultant> Consultants { get; set; } = new List<Consultant>();
}
