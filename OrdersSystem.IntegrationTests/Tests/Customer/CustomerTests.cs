using OrdersSystem.Domain.Models.Dto;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;
using OrdersSystem.IntegrationTests.Helper.DataManipulation;
using OrdersSystem.IntegrationTests.Helper.Prioritize;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OrdersSystem.IntegrationTests.Tests.Customer
{
    [TestCaseOrderer("OrdersSystem.IntegrationTests.Helper.Prioritize.PriorityOrderer", "OrdersSystem.IntegrationTests")]
    public class CustomerTests : IClassFixture<CustomerFixture>
    {
        private const string CreateOrderPath = "/api/customers/create_order";
        private const string CancelOrderPath = "/api/customers/cancel_order/{0}";
        private const string GetOrderPath = "/api/customers/get_order/{0}";
        private const string GetStockPath = "/api/customers/get_stock";
        private readonly CustomerFixture _testFixture;

        public CustomerTests(CustomerFixture customerFixture)
        {
            _testFixture = customerFixture;
        }

        [Theory, TestPriority(1)]
        [MemberData(nameof(CustomerData.CorrectOrderItems), MemberType = typeof(CustomerData))]
        public async Task CreateOrder_ReturnsUnauthorized_WhenNoTokenPassed(IEnumerable<OrderItem> orderItems)
        {
            await RefreshData.RefreshForCustomerTestsAsync(_testFixture.Client);

            var request = new HttpRequestMessage(HttpMethod.Post, CreateOrderPath);
            request.Content = JsonContent.Create(orderItems);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory, TestPriority(2)]
        [MemberData(nameof(CustomerData.CorrectOrderItems), MemberType = typeof(CustomerData))]
        public async Task CreateOrder_ReturnsUnauthorized_WhenWrongTokenPassed(IEnumerable<OrderItem> orderItems)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, CreateOrderPath);
            request.Content = JsonContent.Create(orderItems);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "SomeInvalidTokenValue");
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory, TestPriority(3)]
        [MemberData(nameof(CustomerData.IncorrectOrderItems), MemberType = typeof(CustomerData))]
        public async Task CreateOrder_ReturnsBadRequest_WhenOrderInvalid(IEnumerable<OrderItem> orderItems)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, CreateOrderPath);
            request.Content = JsonContent.Create(orderItems);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory, TestPriority(4)]
        [MemberData(nameof(CustomerData.CorrectOrderItems), MemberType = typeof(CustomerData))]
        public async Task CreateOrder_ReturnsCreated_WhenValidTokenPassed(IEnumerable<OrderItem> orderItems)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, CreateOrderPath);
            request.Content = JsonContent.Create(orderItems);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            _testFixture.OrderId = response.Headers.Location?.Segments.Last();
            var ex = Record.Exception(() => new Guid(_testFixture.OrderId));
            
            Assert.Null(ex);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory, TestPriority(5)]
        [MemberData(nameof(CustomerData.CorrectOrderItems), MemberType = typeof(CustomerData))]
        public async Task CreateOrder_ReturnsNotImplemented_WhenCustomerHasActiveOrder(IEnumerable<OrderItem> orderItems)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, CreateOrderPath);
            request.Content = JsonContent.Create(orderItems);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [Fact, TestPriority(6)]
        public async Task CancelOrder_ReturnsBadRequest_WhenGuidInvalid()
        {
            var url = string.Format(CancelOrderPath, "000-0-12345");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact, TestPriority(7)]
        public async Task CancelOrder_ReturnsNotFound_WhenNoSuchOrder()
        {
            var url = string.Format(CancelOrderPath, Guid.NewGuid());
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact, TestPriority(7)]
        public async Task CancelOrder_ReturnsForbidden_WhenTryCancelSomeoneElseOrder()
        {
            var url = string.Format(CancelOrderPath, "61A396CD-DA3E-5048-9BCD-3A849772379E");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact, TestPriority(8)]
        public async Task CancelOrder_ReturnsNotImplemented_WhenTryCancelFinishedOrder()
        {
            var url = string.Format(CancelOrderPath, "CC702242-895D-3EFA-BBD5-61268913AABC");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [Fact, TestPriority(9)]
        public async Task CancelOrder_ReturnsOk_WhenOrderCancelled()
        {
            var url = string.Format(CancelOrderPath, _testFixture.OrderId);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact, TestPriority(10)]
        public async Task GetByGuidAsync_ReturnsNotFound_WhenNoSuchOrder()
        {
            var url = string.Format(GetOrderPath, Guid.NewGuid());
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact, TestPriority(11)]
        public async Task GetByGuidAsync_ReturnsForbidden_WhenTryGetSomeoneElseOrder()
        {
            var url = string.Format(GetOrderPath, "B006F4FC-9D0B-3D0E-97B5-E229F8EB520D");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact, TestPriority(12)]
        public async Task GetByGuidAsync_ReturnsOrder_WhenTryGetYourOrder()
        {
            var url = string.Format(GetOrderPath, "CC702242-895D-3EFA-BBD5-61268913AABC");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();
            Assert.NotNull(orderDto);
        }

        [Fact, TestPriority(13)]
        public async Task GetStock_ReturnsStock()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetStockPath);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var stock = await response.Content.ReadFromJsonAsync<IEnumerable<StockItem>>();
            
            Assert.Equal(20, stock?.Count());
        }
    }
}
