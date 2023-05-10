using OrdersSystem.Data.Access.Context;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.DataRefresh
{
    public class DbSetsRefresher : IDbSetsRefresher
    {
        private readonly ApplicationContext _applicationContext;
        private readonly IDataGenerator _dataGenerator;

        public DbSetsRefresher(ApplicationContext applicationContext, IDataGenerator dataGenerator)
        {
            _applicationContext = applicationContext;
            _dataGenerator = dataGenerator;
        }

        public void Refresh()
        {
            ClearEntityDbSet<User>();
            ClearEntityDbSet<Customer>();
            ClearEntityDbSet<OrderPicker>();
            ClearEntityDbSet<Order>();
            ClearEntityDbSet<StockItem>();
            ClearEntityDbSet<ReserveItem>();
            ClearEntityDbSet<Sku>();

            _dataGenerator.InitData();

            SeedEntity(_dataGenerator.Users);
            SeedEntity(_dataGenerator.Skus);
            SeedEntity(_dataGenerator.StockItems);
            SeedEntity(_dataGenerator.Customers);
            SeedEntity(_dataGenerator.OrderPickers);
            SeedEntity(_dataGenerator.Orders);
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
