namespace OrdersSystem.Data.Process.Options
{
    public class FakerOptions
    {
        public const string Section = "FakerOptions";

        public int NumPickers { get; set; }
        public int NumCustomers { get; set; }
        public int NumSkus { get; set; }
        public int NumOrderItems { get; set; }
        public int NumOrders { get; set; }
    }
}
