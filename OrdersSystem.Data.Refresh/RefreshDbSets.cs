using OrdersSystem.Data.Access.Context;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Refresh
{
    public class RefreshDbSets : IRefreshDbSets
    {
        private readonly ApplicationContext _applicationContext;

        public RefreshDbSets(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
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

            DataGenerator.InitData();

            SeedEntity(DataGenerator.Users);
            SeedEntity(DataGenerator.Skus);
            SeedEntity(DataGenerator.StockItems);
            SeedEntity(DataGenerator.Customers);
            SeedEntity(DataGenerator.OrderPickers);
            SeedEntity(DataGenerator.Orders);
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