using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.DataRefresh
{
    public interface IDataGenerator
    {
        List<User> Users { get; }
        List<Sku> Skus { get; }
        List<StockItem> StockItems { get; }
        List<Customer> Customers { get; }
        List<OrderPicker> OrderPickers { get; }
        List<OrderItem> OrderItems { get; }
        List<Order> Orders { get; }
        void InitData();
    }
}
