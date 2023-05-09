using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.Api.Auth.Services
{
    public interface IUserService
    {
        Task<User?> GetByUsernameConsideringPasswordAsync(LoginModel loginModel);

        Task<User?> GetByUsernameAsync(string username);
    }
}
