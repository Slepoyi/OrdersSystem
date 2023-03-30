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

        public ValidationResult ValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<StockItem> stockItems)
        {
            return _orderValidator.ValidateOrder(orderItems, stockItems);
        }

        public async Task<Order?> GetByGuidAsync(Guid id)
        {
            return await _applicationContext.Orders
                .Where(o => o.Id == id)
                .Include(o => o.Customer)
                .Include(o => o.OrderPicker)
                .SingleOrDefaultAsync();
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
            catch (Exception)
            {
            
            }

            return stock;
        }

        public async Task<Order?> CreateOrderAsync(IEnumerable<OrderItem> orderItems, Guid userGuid, IEnumerable<StockItem> stockItems)
        {
            Customer? customer = default;
            try
            {
                customer = _applicationContext.Customers.Find(userGuid);
            }
            catch (Exception) { }
            if (customer is null)
                return null;

            var order = new Order(customer, _clock.Now, orderItems);

            if (!await ReserveOrderInDatabaseAsync(order, stockItems))
                return null;

            return order;
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

        private async Task AddOrderReserveAsync(Order order, IEnumerable<StockItem> stockItems)
        {
            foreach (var stockItem in stockItems)
            {
                var orderItem = order.OrderItems.Where(s => s.Sku == stockItem.Sku).FirstOrDefault();
                stockItem.ReduceBalance(orderItem.Amount);
                await _applicationContext.SaveChangesAsync();
            }
        }

        private async Task<bool> DeleteOrderReserveAsync(Order order)
        {
            var stockItems = GetStockForOrderItems(order.OrderItems);

            foreach (var stockItem in stockItems)
            {
                var orderItem = order.OrderItems.Where(s => s.Sku == stockItem.Sku).FirstOrDefault();
                stockItem.IncreaseBalance(orderItem.Amount);
                await _applicationContext.SaveChangesAsync();
            }

            return true;
        }

        private async Task<Order?> GetOrderWithEarliestOpenDateAsync()
        {
            return await _applicationContext.Orders.OrderBy(o => o.OpenTime).FirstOrDefaultAsync();
        }

        public bool CloseOrder(DateTime closeTime, Order order)
        {
            if (order.OrderStatus == OrderStatus.Finished)
                return false;

            order.OrderStatus = OrderStatus.Finished;
            order.CloseTime = closeTime;
            return true;
        }
    //    I am writing an ordering system.I have 2 classes: `OrderItem` represents an item which user tries wants to order and `StockItem` which represents stock balance for the sku. Code of the above mentioned classes is below:

    //public class OrderItem
    //    {
    //        [Required]
    //        public Sku Sku { get; private set; }
    //        [Required]
    //        public uint Amount { get; private set; }
    //    }

    //    public class StockItem
    //    {
    //        [Required]
    //        public Sku Sku { get; private set; }
    //        [Required]
    //        public uint StockBalance { get; private set; }
    //    }

    //    I get IEnumerable<OrderItem> from a customer.Next, I need to validate the order and as a step of validation I need to check if `OrderItem.Sku` is presented on stock.If everything is fine I want to change
    }
}
