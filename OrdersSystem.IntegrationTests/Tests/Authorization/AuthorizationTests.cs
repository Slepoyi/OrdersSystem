using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using OrdersSystem.Domain.Models.Auth;
using System.Net;
using System.Net.Http.Json;

namespace OrdersSystem.IntegrationTests.Tests.Authorization
{
    public class AuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthorizationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [MemberData(nameof(AuthorizationData.CorrectExistingData), MemberType = typeof(AuthorizationData))]
        public async Task Authenticate_ReturnsJwtToken_WhenUserCanBeAuthenticated(LoginModel loginModel)
        {
            var response = await _client.PostAsJsonAsync("/api/auth/", loginModel);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [MemberData(nameof(AuthorizationData.CoorectUnexistingData), MemberType = typeof(AuthorizationData))]
        public async Task Authenticate_ReturnsBadRequest_WhenLoginOrPasswordAreIncorrect(LoginModel loginModel)
        {
            var response = await _client.PostAsJsonAsync("/api/auth/", loginModel);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(AuthorizationData.IncorrectData), MemberType = typeof(AuthorizationData))]
        public async Task Authenticate_ReturnsBadRequest_WhenModelIsInvalid(LoginModel loginModel)
        {
            var response = await _client.PostAsJsonAsync("/api/auth/", loginModel);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
