namespace OrdersSystem.Data.Process.DataRefresh
{
    public interface IDbSetsRefresher
    {
        void RefreshForCustomerTests();

        Task RefreshForPickerTestsAsync();
    }
}
