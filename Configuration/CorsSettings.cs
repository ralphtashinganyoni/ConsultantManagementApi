namespace ConsultantManagementApi.Configuration;

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = ["http://localhost:3000", "http://localhost:5000"];
}
