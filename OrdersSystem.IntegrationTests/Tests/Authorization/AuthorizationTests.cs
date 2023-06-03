using Microsoft.AspNetCore.Mvc.Testing;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.IntegrationTests.Helper.DataManipulation;
using System.Net;
using System.Net.Http.Json;

namespace OrdersSystem.IntegrationTests.Tests.Authorization
{
    public class AuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string RequestPath = "/api/auth/";
        private readonly HttpClient _client;
        public AuthorizationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [MemberData(nameof(AuthorizationData.CorrectExistingData), MemberType = typeof(AuthorizationData))]
        public async Task Authenticate_ReturnsJwtToken_WhenUserCanBeAuthenticated(LoginModel loginModel)
        {
            var response = await _client.PostAsJsonAsync(RequestPath, loginModel);

            var ex = Record.Exception(() => response.EnsureSuccessStatusCode());
            Assert.Null(ex);

            var token = await response.Content.ReadFromJsonAsync<TokenModel>();
            Assert.NotNull(token?.Token);
        }

        [Theory]
        [MemberData(nameof(AuthorizationData.CoorectUnexistingData), MemberType = typeof(AuthorizationData))]
        public async Task Authenticate_ReturnsBadRequest_WhenLoginOrPasswordAreIncorrect(LoginModel loginModel)
        {
            var response = await _client.PostAsJsonAsync(RequestPath, loginModel);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(AuthorizationData.IncorrectData), MemberType = typeof(AuthorizationData))]
        public async Task Authenticate_ReturnsBadRequest_WhenModelIsInvalid(LoginModel loginModel)
        {
            var response = await _client.PostAsJsonAsync(RequestPath, loginModel);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
