using Microsoft.AspNetCore.Mvc.Testing;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.IntegrationTests.Helper.DataManipulation;

namespace OrdersSystem.IntegrationTests.Tests
{
    public abstract class SharedFixture : IDisposable
    {
        public IOrderFlowManager OrderManager { get; set; }
        public HttpClient Client { get; set; }
        public string? OrderId { get; set; }
        public string? Token { get; set; }
        protected abstract LoginModel Model { get; set; }
        public SharedFixture(WebApplicationFactory<Program> factory, IOrderFlowManager orderManager)
        {
            Client = factory.CreateClient();
            OrderManager = orderManager;
            Token = JwtHelper.GetTokenAsync(Client, Model).Result;
        }

    public void Dispose()
        {
            Client.Dispose();
        }
    }
}
