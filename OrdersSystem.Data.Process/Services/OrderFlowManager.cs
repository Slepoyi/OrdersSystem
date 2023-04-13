using Microsoft.EntityFrameworkCore;
using OrdersSystem.Data.Access.Context;
using OrdersSystem.Data.Process.Validation;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;
using OrdersSystem.Domain.Time;

namespace OrdersSystem.Data.Process.Services
{
    public class OrderFlowManager : IOrderFlowManager
    {
        private readonly ApplicationContext _applicationContext;
        private readonly IOrderValidator _orderValidator;
        private readonly IClock _clock;

        public OrderFlowManager(ApplicationContext applicationContext, IOrderValidator orderValidator, IClock clock)
        {
            _applicationContext = applicationContext;
            _orderValidator = orderValidator;
            _clock = clock;
        }

        public ValidationResult ValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<StockItem> stockItems)
        {
            return _orderValidator.ValidateOrder(orderItems, stockItems);
        }

        public async Task<Order?> GetByGuidAsync(Guid id)
        {
            return await _applicationContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderPicker)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public IEnumerable<StockItem> GetStockForOrderItems(IEnumerable<OrderItem> orderItems)
        {
            IEnumerable<StockItem> stock = Enumerable.Empty<StockItem>();
            try
            {
                stock = _applicationContext.StockItems
                .Join(orderItems,
                s => s.Sku,
                o => o.Sku,
                (s, o) => s);
            }
            catch (Exception) { }

            return stock;
        }

        public Task<Order?> GetNextOrderAsync()
        {
            return _applicationContext.Orders.OrderBy(o => o.OpenTime).FirstOrDefaultAsync();
        }

        public async Task<bool> BeginOrderPickingAsync(Order order, Guid userGuid)
        {
            OrderPicker? picker = default;
            try
            {
                picker = await _applicationContext.OrderPickers.FindAsync(userGuid);
            }
            catch (Exception) { }
            if (picker is null)
                return false;

            order.OrderPicker = picker;
            order.OrderStatus = OrderStatus.Processing;
            order.PickingStartTime = _clock.Now;
            await _applicationContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CloseOrder(Order order, Guid userGuid)
        {
            OrderPicker? picker = default;
            try
            {
                picker = await _applicationContext.OrderPickers.FindAsync(userGuid);
            }
            catch (Exception) { }
            if (picker is null)
                return false;

            if (order.OrderStatus != OrderStatus.Processing)
                return false;

            order.OrderStatus = OrderStatus.Finished;
            order.CloseTime = _clock.Now;
            return true;
        }

        public async Task<Order?> CreateOrderAsync(List<OrderItem> orderItems, Guid userGuid, IEnumerable<StockItem> stockItems)
        {
            Customer? customer = default;
            try
            {
                customer = await _applicationContext.Customers.FindAsync(userGuid);
            }
            catch (Exception) { }
            if (customer is null)
                return null;

            var order = new Order(customer, _clock.Now, orderItems);

            if (!await ReserveOrderInDatabaseAsync(order, stockItems))
                return null;

            return order;
        }

        public async Task<bool> CancelOrderAsync(Order order)
        {
            if (order.OrderStatus != OrderStatus.Created)
                return false;

            if (! await DeleteOrderReserveAsync(order))
                return false;

            try
            {
                _applicationContext.Orders.Remove(order);
                await _applicationContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> ReserveOrderInDatabaseAsync(Order order, IEnumerable<StockItem> stockItems)
        {
            if (!AddOrderToDatabase(order))
                return false;

            await AddOrderReserveAsync(order, stockItems);

            return true;
        }
        private bool AddOrderToDatabase(Order order)
        {
            try
            {
                _applicationContext.Orders.Add(order);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task AddOrderReserveAsync(Order order, IEnumerable<StockItem> stockItems)
        {
            foreach (var stockItem in stockItems)
            {
                var orderItem = order.OrderItems.FirstOrDefault(s => s.Sku == stockItem.Sku);
                stockItem.ReduceBalance(orderItem.Quantity);
                await _applicationContext.SaveChangesAsync();
            }
        }

        private async Task<bool> DeleteOrderReserveAsync(Order order)
        {
            var stockItems = GetStockForOrderItems(order.OrderItems);

            foreach (var stockItem in stockItems)
            {
                var orderItem = order.OrderItems.FirstOrDefault(s => s.Sku == stockItem.Sku);
                stockItem.IncreaseBalance(orderItem.Quantity);
                await _applicationContext.SaveChangesAsync();
            }

            return true;
        }
    }
}
