﻿using OrdersSystem.Data.Access.Context;
using OrdersSystem.Domain.Helper;
using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.Api.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext _applicationContext;

        public UserService(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<User?> GetByUsernameConsideringPasswordAsync(LoginModel loginModel)
        {
            var user = await GetByUsernameAsync(loginModel.Username);

            if (!CheckPasswordHash(user?.Password, loginModel.Password))
                return null;

            return user;
        }

        public async Task<User?> GetByUsernameAsync(string username)
            => await _applicationContext.Users.FindAsync(username);

        private static bool CheckPasswordHash(string? passwordHash, string password)
        {
            if (passwordHash is null)
                return false;

            return passwordHash == password;
        }
    }
}
