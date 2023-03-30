using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.Api.Auth.Services
{
    public interface IUserService
    {
        public User? GetCurrentAuthenticatedUser();

        Task<User?> GetByIdConsideringPasswordAsync(Guid id, string password);

        Task<User?> GetByIdAsync(Guid id);
    }
}
