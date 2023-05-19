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
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public IEnumerable<StockItem> GetStockForOrderItems(IEnumerable<OrderItem> orderItems)
        {
            var stock = _applicationContext.StockItems
                .AsEnumerable()
                .Join(orderItems,
                s => s.SkuId,
                o => o.SkuId,
                (s, _) => s)
                .ToList();

            return stock;
        }

        public IEnumerable<StockItem> GetStock()
        {
            return _applicationContext.StockItems.Include(s => s.Sku).AsEnumerable();
        }

        public Task<Order?> GetNextOrderAsync()
        {
            return _applicationContext.Orders.OrderBy(o => o.OpenTime).FirstOrDefaultAsync();
        }

        public async Task<bool> BeginOrderPickingAsync(Order order, Guid userGuid)
        {
            var picker = await _applicationContext.OrderPickers.FindAsync(userGuid);
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
            var picker = await _applicationContext.OrderPickers.FindAsync(userGuid);
            if (picker is null)
                return false;

            if (order.OrderStatus != OrderStatus.Processing)
                return false;

            await RollbackOrderReservePart(order);

            order.OrderStatus = OrderStatus.Finished;
            order.CloseTime = _clock.Now;
            return true;
        }

        public async Task<Order?> CreateOrderAsync(List<OrderItem> orderItems, Guid userGuid, IEnumerable<StockItem> stockItems)
        {
            var customer = await _applicationContext.Customers.FindAsync(userGuid);
            if (customer is null)
                return null;

            var order = new Order
            {
                Customer = customer,
                OpenTime = _clock.Now,
                OrderItems = orderItems
            };
            foreach (var item in order.OrderItems)
            {
                item.OrderId = order.Id;
            }

            await ReserveOrderInDatabaseAsync(order, stockItems);

            return order;
        }

        public async Task<bool> CancelOrderAsync(Order order)
        {
            if (order.OrderStatus != OrderStatus.Created)
                return false;

            DeleteOrderReserve(order);

            _applicationContext.Orders.Remove(order);
            await _applicationContext.SaveChangesAsync();
            return true;
        }

        private async Task<bool> ReserveOrderInDatabaseAsync(Order order, IEnumerable<StockItem> stockItems)
        {
            await _applicationContext.Orders.AddAsync(order);
            await _applicationContext.OrderItems.AddRangeAsync(order.OrderItems);
            await AddOrderReserveAsync(order, stockItems);
            await _applicationContext.SaveChangesAsync();

            return true;
        }

        private async Task<bool> AddOrderReserveAsync(Order order, IEnumerable<StockItem> stockItems)
        {
            foreach (var stockItem in stockItems)
            {
                var orderItem = order.OrderItems.FirstOrDefault(s => s.SkuId == stockItem.SkuId);
                var reserveItem = await _applicationContext.ReservedItems.FirstOrDefaultAsync(s => s.SkuId == orderItem.SkuId);
                stockItem.ReduceBalance(orderItem.Quantity);
                if (reserveItem is null)
                    await _applicationContext.ReservedItems.AddAsync(
                        new ReserveItem
                        {
                            Sku = orderItem.Sku,
                            SkuId = orderItem.SkuId,
                            StockBalance = orderItem.Quantity
                        });
                else
                    reserveItem.StockBalance += orderItem.Quantity;
            }
            return true;
        }

        private void DeleteOrderReserve(Order order)
        {
            var stockItems = GetStockForOrderItems(order.OrderItems);
            foreach (var stockItem in stockItems)
            {
                var orderItem = order.OrderItems.FirstOrDefault(s => s.SkuId == stockItem.SkuId);
                stockItem.IncreaseBalance(orderItem.Quantity);
            }
        }

        private async Task RollbackOrderReservePart(Order order)
        {
            foreach (var orderItem in order.OrderItems)
            {
                var reserveItem = await _applicationContext.ReservedItems.FirstOrDefaultAsync(s => s.SkuId == orderItem.SkuId);
                reserveItem.StockBalance -= orderItem.Quantity;
            }
        }
    }
}
