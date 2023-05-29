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

        public ValidationResult ValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<IItem> items)
        {
            return _orderValidator.ValidateOrder(orderItems, items);
        }

        public bool CustomerHasOpenedOrder(Guid id)
        {
            var order = _applicationContext.Orders
                .FirstOrDefault(o => o.CustomerId == id && o.OrderStatus != OrderStatus.Finished);

            return order is not null;
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
            return _applicationContext.StockItems
                .AsEnumerable()
                .Join(orderItems,
                s => s.SkuId,
                o => o.SkuId,
                (s, _) => s)
                .ToList();
        }

        public IEnumerable<ReserveItem> GetReserveForOrderItems(IEnumerable<OrderItem> orderItems)
        {
            return _applicationContext.ReservedItems
                .AsEnumerable()
                .Join(orderItems,
                r => r.SkuId,
                o => o.SkuId,
                (r, _) => r)
                .ToList();
        }

        public IEnumerable<StockItem> GetStock()
        {
            return _applicationContext.StockItems.Include(s => s.Sku).AsEnumerable();
        }

        public Task<Order?> GetNextOrderAsync()
        {
            return _applicationContext.Orders.
                OrderBy(o => o.OpenTime).
                Include(o => o.OrderItems).
                FirstOrDefaultAsync(o => o.OrderStatus == OrderStatus.Created);
        }

        public async Task<bool> BeginOrderPickingAsync(Order order, Guid userGuid)
        {
            var picker = await _applicationContext.OrderPickers.FindAsync(userGuid);
            if (picker is null)
                return false;

            order.OrderPickerId = picker.Id;
            order.OrderStatus = OrderStatus.Processing;
            order.PickingStartTime = _clock.Now;
            await _applicationContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CloseOrder(Order order, Guid userGuid, IEnumerable<OrderItem> orderItems)
        {
            var picker = await _applicationContext.OrderPickers.FindAsync(userGuid);
            if (picker is null)
                return false;

            if (order.OrderStatus != OrderStatus.Processing)
                return false;

            await RollbackOrderReservePartAsync(order);

            foreach (var item in order.OrderItems)
            {
                var oi = orderItems.FirstOrDefault(i => i.Id == item.Id);
                item.Quantity = oi.Quantity;
            }

            order.OrderStatus = OrderStatus.Finished;
            order.CloseTime = _clock.Now;
            await _applicationContext.SaveChangesAsync();
            return true;
        }

        public async Task<Order?> CreateOrderAsync(IEnumerable<OrderItem> orderItems, Guid userGuid, IEnumerable<StockItem> stockItems)
        {
            var customer = await _applicationContext.Customers.FindAsync(userGuid);
            if (customer is null)
                return null;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                OpenTime = _clock.Now,
            };
            foreach (var item in orderItems)
            {
                item.Id = Guid.NewGuid();
                item.OrderId = order.Id;
            }

            await ReserveOrderInDatabaseAsync(order, stockItems, orderItems);

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

        private async Task<bool> ReserveOrderInDatabaseAsync(Order order, IEnumerable<StockItem> stockItems, IEnumerable<OrderItem> orderItems)
        {
            await _applicationContext.Orders.AddAsync(order);
            await _applicationContext.OrderItems.AddRangeAsync(orderItems);
            await AddOrderReserveAsync(stockItems, orderItems);
            await _applicationContext.SaveChangesAsync();

            return true;
        }

        private async Task<bool> AddOrderReserveAsync(IEnumerable<StockItem> stockItems, IEnumerable<OrderItem> orderItems)
        {
            foreach (var stockItem in stockItems)
            {
                var orderItem = orderItems.FirstOrDefault(s => s.SkuId == stockItem.SkuId);
                var reserveItem = await _applicationContext.ReservedItems.FirstOrDefaultAsync(s => s.SkuId == orderItem.SkuId);
                stockItem.Quantity -= orderItem.Quantity;
                if (reserveItem is null)
                    await _applicationContext.ReservedItems.AddAsync(
                        new ReserveItem
                        {
                            Sku = orderItem.Sku,
                            SkuId = orderItem.SkuId,
                            Quantity = orderItem.Quantity
                        });
                else
                    reserveItem.Quantity += orderItem.Quantity;
            }
            return true;
        }

        private void DeleteOrderReserve(Order order)
        {
            var stockItems = GetStockForOrderItems(order.OrderItems);
            foreach (var stockItem in stockItems)
            {
                var orderItem = order.OrderItems.FirstOrDefault(s => s.SkuId == stockItem.SkuId);
                stockItem.Quantity += orderItem.Quantity;
            }
        }

        private async Task RollbackOrderReservePartAsync(Order order)
        {
            foreach (var orderItem in order.OrderItems)
            {
                var reserveItem = await _applicationContext.ReservedItems.FirstOrDefaultAsync(s => s.SkuId == orderItem.SkuId);
                var balance = reserveItem.Quantity - orderItem.Quantity;
                if (balance == 0)
                    _applicationContext.ReservedItems.Remove(reserveItem);
                else
                    reserveItem.Quantity -= orderItem.Quantity;
            }
        }
    }
}
