using OrdersSystem.Data.Access.Context;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.DataRefresh
{
    public class DbSetsRefresher : IDbSetsRefresher
    {
        private readonly ApplicationContext _applicationContext;
        private readonly IDataGenerator _dataGenerator;
        private readonly IOrderFlowManager _orderFlowManager;

        public DbSetsRefresher(ApplicationContext applicationContext, IDataGenerator dataGenerator,
            IOrderFlowManager orderFlowManager)
        {
            _orderFlowManager = orderFlowManager;
            _applicationContext = applicationContext;
            _dataGenerator = dataGenerator;
        }

        public async Task RefreshForPickerTestsAsync()
        {
            RefreshForCustomerTests();
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    SkuId = new Guid("B9062C62-9A5D-A0FB-CDBA-EB80445E1187"),
                    Quantity = 6
                },
                new OrderItem
                {
                    SkuId = new Guid("613DCD97-383E-ADD6-4E28-337396AD9585"),
                    Quantity = 4
                },
                new OrderItem
                {
                    SkuId = new Guid("833C33B0-35A1-84B3-01B6-68F725707101"),
                    Quantity = 2
                }
            };
            var stock = _orderFlowManager.GetStockForOrderItems(orderItems);
            var result = await _orderFlowManager.CreateOrderAsync(orderItems, new Guid("CE9EC503-29C7-1775-741C-27AED0FB0850"), stock);
        }

        public void RefreshForCustomerTests()
        {
            ClearEntityDbSet<User>();
            ClearEntityDbSet<Customer>();
            ClearEntityDbSet<OrderPicker>();
            ClearEntityDbSet<Order>();
            ClearEntityDbSet<StockItem>();
            ClearEntityDbSet<ReserveItem>();
            ClearEntityDbSet<Sku>();
            ClearEntityDbSet<OrderItem>();

            _dataGenerator.InitData();

            SeedEntity(_dataGenerator.Users);
            SeedEntity(_dataGenerator.Skus);
            SeedEntity(_dataGenerator.StockItems);
            SeedEntity(_dataGenerator.Customers);
            SeedEntity(_dataGenerator.OrderPickers);
            SeedEntity(_dataGenerator.Orders);
            SeedEntity(_dataGenerator.OrderItems);
        }

        private void ClearEntityDbSet<T>() where T : class, new()
        {
            var entities = _applicationContext.Set<T>();
            if (entities is null)
                return;
            _applicationContext.Set<T>().RemoveRange(entities);
            _applicationContext.SaveChanges();
        }
        private void SeedEntity<T>(List<T> values) where T : class, new()
        {
            _applicationContext.Set<T>().AddRange(values);
            _applicationContext.SaveChanges();
        }
    }
}
