using OrdersSystem.Domain.Models.Auth;
using System.Net.Http.Json;

namespace OrdersSystem.IntegrationTests.Helper.DataManipulation
{
    public class JwtHelper
    {
        public static async Task<string?> GetTokenAsync(HttpClient client, LoginModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/");
            request.Content = JsonContent.Create(model);
            var response = await client.SendAsync(request);
            var token = await response.Content.ReadFromJsonAsync<TokenModel>();
            return token?.Token;
        }
    }
}
