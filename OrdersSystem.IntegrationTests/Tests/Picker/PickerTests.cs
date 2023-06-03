using Microsoft.AspNetCore.Mvc.Testing;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Dto;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.IntegrationTests.Helper.DataManipulation;
using OrdersSystem.IntegrationTests.Helper.Prioritize;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OrdersSystem.IntegrationTests.Tests.Picker
{
    [TestCaseOrderer("OrdersSystem.IntegrationTests.Helper.Prioritize.PriorityOrderer", "OrdersSystem.IntegrationTests")]
    public class PickerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string FinishOrderPath = "/api/pickers/finish/{0}";
        private const string AssignOrderPath = "/api/pickers/assign";
        private readonly IOrderFlowManager _orderManager;
        private readonly HttpClient _client;
        private readonly string? _token;
        private readonly string _freshOrderId;
        public PickerTests(IOrderFlowManager orderFlowManager, WebApplicationFactory<Program> factory)
        {
            _orderManager = orderFlowManager;
            _client = factory.CreateClient();
            _token = JwtHelper.GetTokenAsync(_client, new LoginModel
            {
                Username = "Arthur_Bayer",
                Password = "xt33OZOb"
            }).Result;
        }

        [Fact, TestPriority(1)]
        public async Task AssignOrder_ReturnsNotFound_WhenNoOrderAvailable()
        {
            await RefreshData.RefreshForCustomerTestsAsync(_client);

            var request = new HttpRequestMessage(HttpMethod.Post, AssignOrderPath);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact, TestPriority(2)]
        public async Task AssignOrder_ReturnsOk_WhenOrderAvailable()
        {
            await RefreshData.RefreshForPickerTestsAsync(_client);

            var request = new HttpRequestMessage(HttpMethod.Post, AssignOrderPath);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await _client.SendAsync(request);

            var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(OrderStatus.Processing, orderDto?.OrderStatus);
            Assert.NotEqual(DateTime.MinValue, orderDto?.PickingStartTime);
        }

        [Theory, TestPriority(3)]
        [MemberData(nameof(PickerData.IncorrectOrderItems), MemberType = typeof(PickerData))]
        public async Task FinishOrder_ReturnsBadRequest_WhenOrderInvalid(IEnumerable<OrderItem> orderItems)
        {
            var url = string.Format(FinishOrderPath, _freshOrderId);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Content = JsonContent.Create(orderItems);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory, TestPriority(4)]
        [MemberData(nameof(PickerData.CorrectOrderItems), MemberType = typeof(PickerData))]
        public async Task FinishOrder_ReturnsForbidden_WhenOrderNotBelongToPicker(IEnumerable<OrderItem> orderItems)
        {
            await RefreshData.RefreshForPickerTestsAsync(_client);
            var order = await _orderManager.GetNextOrderAsync();
            Assert.NotNull(order);

            await _orderManager.BeginOrderPickingAsync(order, new Guid("1BC67FB0-8D10-91F2-BF50-CFB0E7552417"));

            var url = string.Format(FinishOrderPath, order.Id);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Content = JsonContent.Create(orderItems);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory, TestPriority(5)]
        [MemberData(nameof(PickerData.CorrectOrderItems), MemberType = typeof(PickerData))]
        public async Task FinishOrder_ReturnsNotFound_WhenNoOrderWithThisId(IEnumerable<OrderItem> orderItems)
        {
            var url = string.Format(FinishOrderPath, "B9062C62-9A5D-A0FB-CDBA-EB80445E1187");

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Content = JsonContent.Create(orderItems);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory, TestPriority(6)]
        [MemberData(nameof(PickerData.CorrectOrderItems), MemberType = typeof(PickerData))]
        public async Task FinishOrder_ReturnsOk_WhenPassedCorrectOrder(IEnumerable<OrderItem> orderItems)
        {
            await RefreshData.RefreshForPickerTestsAsync(_client);
            var order = await _orderManager.GetNextOrderAsync();
            Assert.NotNull(order);

            await _orderManager.BeginOrderPickingAsync(order, new Guid("EA6908D8-8F45-6F5F-A9FF-22A4050FF300"));

            var url = string.Format(FinishOrderPath, order.Id);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Content = JsonContent.Create(orderItems);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory, TestPriority(7)]
        [MemberData(nameof(PickerData.CorrectOrderItemsQuantityCutInHalf), MemberType = typeof(PickerData))]
        public async Task FinishOrder_ReturnsOk_WhenPassedCorrectOrderCut(IEnumerable<OrderItem> orderItems)
        {
            await RefreshData.RefreshForPickerTestsAsync(_client);
            var order = await _orderManager.GetNextOrderAsync();
            Assert.NotNull(order);

            await _orderManager.BeginOrderPickingAsync(order, new Guid("EA6908D8-8F45-6F5F-A9FF-22A4050FF300"));

            var url = string.Format(FinishOrderPath, order.Id);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Content = JsonContent.Create(orderItems);
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
