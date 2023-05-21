using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.Validation
{
    public interface IOrderValidator
    {
        ValidationResult CustomerValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<StockItem> stockItems);
        ValidationResult PickerValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<ReserveItem> reserveItems);
    }
}
