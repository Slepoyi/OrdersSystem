using OrdersSystem.Data.Access.Context;
using OrdersSystem.Domain.Helper;
using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.Api.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICryptoProvider _cryptoProvider;
        private readonly ApplicationContext _applicationContext;

        public UserService(IHttpContextAccessor httpContextAccessor, ICryptoProvider cryptoProvider,
            ApplicationContext applicationContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _cryptoProvider = cryptoProvider;
            _applicationContext = applicationContext;
        }

        public User? GetCurrentAuthenticatedUser()
            => _httpContextAccessor.HttpContext.Items["User"] as User;

        public async Task<User?> GetByIdConsideringPasswordAsync(Guid id, string password)
        {
            var user = await GetByIdAsync(id);

            if (!CheckPasswordHash(user, password))
                return null;

            return user;
        }

        public async Task<User?> GetByIdAsync(Guid id)
            => await _applicationContext.Users.FindAsync(id);

        private bool CheckPasswordHash(User? user, string password)
        {
            if (user is null)
                return false;

            return user.Password == _cryptoProvider.CreateCryptoString(password);
        }
    }
}
