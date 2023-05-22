using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.Validation
{
    public interface IOrderValidator
    {
        ValidationResult ValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<IItem> items);
    }
}
