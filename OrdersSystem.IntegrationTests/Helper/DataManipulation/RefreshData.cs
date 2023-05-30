namespace OrdersSystem.IntegrationTests.Helper.DataManipulation
{
    public class RefreshData
    {
        public static async Task RefreshAsync(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/data/refresh");
            await client.SendAsync(request);
        }
    }
}
