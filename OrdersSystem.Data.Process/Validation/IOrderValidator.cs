using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.Validation
{
    internal interface IOrderValidator
    {
        ValidationResult ValidateOrder(IEnumerable<OrderItem> order, IEnumerable<StockItem> stock);
    }
}
