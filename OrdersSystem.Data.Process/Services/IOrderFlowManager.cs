using OrdersSystem.Data.Process.Validation;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.Services
{
    public interface IOrderFlowManager
    {
        ValidationResult ValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<IItem> items);
        IEnumerable<ReserveItem> GetReserveForOrderItems(IEnumerable<OrderItem> orderItems);
        IEnumerable<StockItem> GetStockForOrderItems(IEnumerable<OrderItem> orderItems);
        IEnumerable<StockItem> GetStock();
        bool CustomerHasOpenedOrder(Guid id);
        Task<Order?> GetByGuidAsync(Guid id);
        Task<Order?> CreateOrderAsync(IEnumerable<OrderItem> orderItems, Guid userGuid, IEnumerable<StockItem> stockItems);
        Task<bool> CancelOrderAsync(Order order);
        Task<Order?> GetNextOrderAsync();
        Task<bool> BeginOrderPickingAsync(Order order, Guid userGuid);
        Task<bool> CloseOrder(Order order, Guid userGuid, IEnumerable<OrderItem> orderItems);
    }
}
