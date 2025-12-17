namespace ConsultantManagementApi.DTOs;

public class ConsultantDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }
    public int ConsultantRoleId { get; set; }
    public string ConsultantRoleName { get; set; } = string.Empty;
    public decimal CurrentRatePerHour { get; set; }
}

public class CreateConsultantRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int ConsultantRoleId { get; set; }
}

public class UpdateConsultantRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int ConsultantRoleId { get; set; }
}
