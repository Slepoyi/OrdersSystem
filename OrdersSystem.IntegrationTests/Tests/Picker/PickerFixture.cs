using Microsoft.AspNetCore.Mvc.Testing;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.IntegrationTests.Helper.DataManipulation;

namespace OrdersSystem.IntegrationTests.Tests.Picker
{
    public class PickerFixture : SharedFixture
    {
        protected override LoginModel Model { get; set; } =
            new LoginModel
            {
                Username = "Arthur_Bayer",
                Password = "xt33OZOb"
            };

        public PickerFixture(
            WebApplicationFactory<Program> factory,
            IOrderFlowManager orderManager) : base(factory, orderManager) { }
    }
}
