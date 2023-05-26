using Microsoft.AspNetCore.Mvc.Testing;
using OrdersSystem.Domain.Models.Auth;
using System.Net.Http.Json;

namespace OrdersSystem.IntegrationTests.Authorization
{
    public class AuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthorizationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        public async Task Authenticate_ReturnsJwtToken_WhenUserCanBeAuthenticated(LoginModel loginModel)
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/auth/", loginModel);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        public async Task Authenticate_ReturnsBadRequest_WhenModelIsInvalid(LoginModel loginModel)
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/auth/", loginModel);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        public async Task Authenticate_ReturnsBadRequest_WhenLoginOrPasswordAreIncorrect(LoginModel loginModel)
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/auth/", loginModel);

            response.EnsureSuccessStatusCode();
        }
    }
}
