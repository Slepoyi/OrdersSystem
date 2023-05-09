using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.Api.Auth.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);

        string? ValidateTokenAndExtractUsername(string? token);
    }
}
