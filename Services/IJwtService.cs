using ConsultantManagementApi.Models;

namespace ConsultantManagementApi.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
