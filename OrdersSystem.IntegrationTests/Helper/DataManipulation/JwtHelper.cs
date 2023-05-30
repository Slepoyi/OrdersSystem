using OrdersSystem.Domain.Models.Auth;
using System.Net.Http.Json;

namespace OrdersSystem.IntegrationTests.Helper.DataManipulation
{
    public class JwtHelper
    {
        public static async Task<string> GetCustomerTokenAsync(HttpClient client)
        {
            var model = new LoginModel { Username = "Aliya_Cruickshank", Password = "PVBCbyoW" };

            return await SendAuthRequestAsync(client, model);
        }

        public static async Task<string> GetPickerTokenAsync(HttpClient client)
        {
            var model = new LoginModel { Username = "Aliya_Cruickshank", Password = "PVBCbyoW" };

            return await SendAuthRequestAsync(client, model);
        }

        private static async Task<string> SendAuthRequestAsync(HttpClient client, LoginModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/");
            request.Content = JsonContent.Create(model);
            var response = await client.SendAsync(request);
            var token = await response.Content.ReadFromJsonAsync<TokenModel>();
            return token.Token;
        }
    }
}
