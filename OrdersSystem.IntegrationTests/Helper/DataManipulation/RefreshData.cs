namespace OrdersSystem.IntegrationTests.Helper.DataManipulation
{
    public class RefreshData
    {
        public static async Task RefreshForCustomerTestsAsync(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/data/_refresh_customer");
            await client.SendAsync(request);
        }

        public static async Task RefreshForPickerTestsAsync(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/data/_refresh_picker");
            await client.SendAsync(request);
        }
    }
}
