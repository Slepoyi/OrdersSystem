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
    public class PickerTests : IClassFixture<PickerFixture>
    {
        private const string FinishOrderPath = "/api/pickers/finish/{0}";
        private const string AssignOrderPath = "/api/pickers/assign";
        private readonly PickerFixture _testFixture;

        public PickerTests(PickerFixture pickerFixture)
        {
            _testFixture = pickerFixture;
        }

        [Fact, TestPriority(1)]
        public async Task AssignOrder_ReturnsNotFound_WhenNoOrderAvailable()
        {
            await RefreshData.RefreshForCustomerTestsAsync(_testFixture.Client);

            var request = new HttpRequestMessage(HttpMethod.Post, AssignOrderPath);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact, TestPriority(2)]
        public async Task AssignOrder_ReturnsOk_WhenOrderAvailable()
        {
            await RefreshData.RefreshForPickerTestsAsync(_testFixture.Client);

            var request = new HttpRequestMessage(HttpMethod.Post, AssignOrderPath);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            var response = await _testFixture.Client.SendAsync(request);

            var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();

            Assert.NotNull(orderDto);

            _testFixture.OrderId = orderDto.Id.ToString();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(OrderStatus.Processing, orderDto.OrderStatus);
            Assert.NotEqual(DateTime.MinValue, orderDto.PickingStartTime);
        }

        [Theory, TestPriority(3)]
        [MemberData(nameof(PickerData.IncorrectOrderItems), MemberType = typeof(PickerData))]
        public async Task FinishOrder_ReturnsBadRequest_WhenOrderInvalid(IEnumerable<OrderItem> orderItems)
        {
            var url = string.Format(FinishOrderPath, _testFixture.OrderId);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            request.Content = JsonContent.Create(orderItems);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory, TestPriority(4)]
        [MemberData(nameof(PickerData.CorrectOrderItems), MemberType = typeof(PickerData))]
        public async Task FinishOrder_ReturnsNotFound_WhenNoOrderWithThisId(IEnumerable<OrderItem> orderItems)
        {
            var url = string.Format(FinishOrderPath, "B9062C62-9A5D-A0FB-CDBA-EB80445E1187");

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            request.Content = JsonContent.Create(orderItems);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory, TestPriority(5)]
        [MemberData(nameof(PickerData.CorrectOrderItems), MemberType = typeof(PickerData))]
        public async Task FinishOrder_ReturnsOk_WhenPassedCorrectOrder(IEnumerable<OrderItem> orderItems)
        {
            var url = string.Format(FinishOrderPath, _testFixture.OrderId);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            request.Content = JsonContent.Create(orderItems);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory, TestPriority(6)]
        [MemberData(nameof(PickerData.CorrectOrderItemsQuantityCutInHalf), MemberType = typeof(PickerData))]
        public async Task FinishOrder_ReturnsOk_WhenPassedCorrectOrderCut(IEnumerable<OrderItem> orderItems)
        {
            await RefreshData.RefreshForPickerTestsAsync(_testFixture.Client);
            var order = await _testFixture.OrderManager.GetNextOrderAsync();
            Assert.NotNull(order);

            await _testFixture.OrderManager.BeginOrderPickingAsync(order, new Guid("EA6908D8-8F45-6F5F-A9FF-22A4050FF300"));

            var url = string.Format(FinishOrderPath, order.Id);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            request.Content = JsonContent.Create(orderItems);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory, TestPriority(7)]
        [MemberData(nameof(PickerData.CorrectOrderItems), MemberType = typeof(PickerData))]
        public async Task FinishOrder_ReturnsForbidden_WhenOrderNotBelongToPicker(IEnumerable<OrderItem> orderItems)
        {
            await RefreshData.RefreshForPickerTestsAsync(_testFixture.Client);
            var order = await _testFixture.OrderManager.GetNextOrderAsync();
            Assert.NotNull(order);

            await _testFixture.OrderManager.BeginOrderPickingAsync(order, new Guid("1BC67FB0-8D10-91F2-BF50-CFB0E7552417"));

            var url = string.Format(FinishOrderPath, order.Id);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testFixture.Token);
            request.Content = JsonContent.Create(orderItems);
            var response = await _testFixture.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
