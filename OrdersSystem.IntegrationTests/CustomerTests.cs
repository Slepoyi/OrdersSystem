using Microsoft.AspNetCore.Mvc.Testing;
using OrdersSystem.Domain.Models.Ordering;
using System.Net.Http.Json;

namespace OrdersSystem.IntegrationTests
{
    public class CustomerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CustomerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        public async Task CreateOrder_ReturnsOk_WhenOrderIsValid(IEnumerable<OrderItem> orderItems)
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/customers/create_order", orderItems);

            response.EnsureSuccessStatusCode();
        }
    }
}