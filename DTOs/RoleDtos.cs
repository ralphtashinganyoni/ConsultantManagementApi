namespace ConsultantManagementApi.DTOs;

public class ConsultantRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal RatePerHour { get; set; }
}

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal RatePerHour { get; set; }
}

public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal RatePerHour { get; set; }
}
