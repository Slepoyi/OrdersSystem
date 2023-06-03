using Microsoft.AspNetCore.Mvc.Testing;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.IntegrationTests.Tests.Customer
{
    public class CustomerFixture : SharedFixture
    {
        protected override LoginModel Model { get; set; } =
            new LoginModel
            {
                Username = "Aliya_Cruickshank",
                Password = "PVBCbyoW"
            };

        public CustomerFixture(
            WebApplicationFactory<Program> factory,
            IOrderFlowManager orderManager) : base(factory, orderManager) { }
    }
}
