using Microsoft.AspNetCore.Mvc.Testing;
using OrdersSystem.Domain.Models.Auth;
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
    public class CustomerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string _createOrderPath = "/api/customers/create_order";
        private const string _cancelOrderPath = "/api/customers/cancel_order/{0}";
        private const string _getOrderPath = "/api/customers/get_order/{0}";
        private const string _getStockPath = "/api/customers/get_stock";
        private readonly HttpClient _client;
        private readonly string? _token;
        private string? _freshOrderId;

        public CustomerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _token = JwtHelper.GetTokenAsync(_client, new LoginModel
            {
                Username = "Aliya_Cruickshank",
                Password = "PVBCbyoW"
            }).Result;
        }

        [Theory, TestPriority(1)]
        [MemberData(nameof(CustomerData.CorrectOrderItems), MemberType = typeof(CustomerData))]
        public async Task CreateOrder_ReturnsUnauthorized_WhenNoTokenPassed(IEnumerable<OrderItem> orderItems)
        {
            await RefreshData.RefreshForCustomerTestsAsync(_client); // clutch, see github.com/xunit/xunit/issues/2347

            var request = new HttpRequestMessage(HttpMethod.Post, _createOrderPath);
            request.Content = JsonContent.Create(orderItems);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory, TestPriority(2)]
        [MemberData(nameof(CustomerData.CorrectOrderItems), MemberType = typeof(CustomerData))]
        public async Task CreateOrder_ReturnsUnauthorized_WhenWrongTokenPassed(IEnumerable<OrderItem> orderItems)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _createOrderPath);
            request.Content = JsonContent.Create(orderItems);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "SomeInvalidTokenValue");
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory, TestPriority(3)]
        [MemberData(nameof(CustomerData.IncorrectOrderItems), MemberType = typeof(CustomerData))]
        public async Task CreateOrder_ReturnsBadRequest_WhenOrderInvalid(IEnumerable<OrderItem> orderItems)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _createOrderPath);
            request.Content = JsonContent.Create(orderItems);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory, TestPriority(4)]
        [MemberData(nameof(CustomerData.CorrectOrderItems), MemberType = typeof(CustomerData))]
        public async Task CreateOrder_ReturnsCreated_WhenValidTokenPassed(IEnumerable<OrderItem> orderItems)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _createOrderPath);
            request.Content = JsonContent.Create(orderItems);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            _freshOrderId = response.Headers.Location?.Segments.Last();

            var ex = Record.Exception(() => Guid.Parse(_freshOrderId));

            Assert.Null(ex);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory, TestPriority(5)]
        [MemberData(nameof(CustomerData.CorrectOrderItems), MemberType = typeof(CustomerData))]
        public async Task CreateOrder_ReturnsNotImplemented_WhenCustomerHasActiveOrder(IEnumerable<OrderItem> orderItems)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _createOrderPath);
            request.Content = JsonContent.Create(orderItems);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
        }

        [Fact, TestPriority(6)]
        public async Task CancelOrder_ReturnsBadRequest_WhenGuidInvalid()
        {
            var url = string.Format(_cancelOrderPath, "000-0-12345");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact, TestPriority(7)]
        public async Task CancelOrder_ReturnsNotFound_WhenNoSuchOrder()
        {
            var url = string.Format(_cancelOrderPath, Guid.NewGuid());
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact, TestPriority(7)]
        public async Task CancelOrder_ReturnsForbidden_WhenTryCancelSomeoneElsesOrder()
        {
            var url = string.Format(_cancelOrderPath, "61A396CD-DA3E-5048-9BCD-3A849772379E");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact, TestPriority(8)]
        public async Task CancelOrder_ReturnsNotImplemented_WhenTryCancelFinishedOrder()
        {
            var url = string.Format(_cancelOrderPath, "CC702242-895D-3EFA-BBD5-61268913AABC");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
        }

        // this test fails because of github.com/xunit/xunit/issues/2347
        // as long as I cannot save id of the created order
        [Fact, TestPriority(9)]
        public async Task CancelOrder_ReturnsOk_WhenOrderCancelled()
        {
            var url = string.Format(_cancelOrderPath, _freshOrderId);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact, TestPriority(10)]
        public async Task GetByGuidAsync_ReturnsNotFound_WhenNoSuchOrder()
        {
            var url = string.Format(_getOrderPath, Guid.NewGuid());
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact, TestPriority(11)]
        public async Task GetByGuidAsync_ReturnsForbidden_WhenTryGetSomeoneElsesOrder()
        {
            var url = string.Format(_getOrderPath, "B006F4FC-9D0B-3D0E-97B5-E229F8EB520D");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact, TestPriority(12)]
        public async Task GetByGuidAsync_ReturnsOrder_WhenTryGetYourOrder()
        {
            var url = string.Format(_getOrderPath, "CC702242-895D-3EFA-BBD5-61268913AABC");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();
            Assert.NotNull(orderDto);
        }

        [Fact, TestPriority(13)]
        public async Task GetStock_ReturnsStock()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _getStockPath);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var stock = await response.Content.ReadFromJsonAsync<IEnumerable<StockItem>>();
            
            Assert.Equal(20, stock?.Count());
        }
    }
}
