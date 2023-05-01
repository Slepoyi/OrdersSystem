using Bogus;
using OrdersSystem.Domain.Enums;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Access.SeedData
{
    public class DataGenerator
    {
        private const int _numPickers = 2;
        private const int _numCustomers = 3;
        private const int _numSkus = 20;
        private const int _numOrderItems = 20;
        private const int _numOrders = 10;
        public static List<User> Users { get; private set; } = new();
        public static List<Sku> Skus { get; private set; } = new();
        public static List<StockItem> StockItems { get; private set; } = new();
        public static List<Customer> Customers { get; private set; } = new();
        public static List<OrderPicker> OrderPickers { get; private set; } = new();
        public static List<OrderItem> OrderItems { get; set; } = new();
        public static List<Order> Orders { get; private set; } = new();

        public static void InitData()
        {
            Users = new List<User>();
            Skus = new List<Sku>();
            StockItems = new List<StockItem>();
            Customers = new List<Customer>();
            OrderPickers = new List<OrderPicker>();
            OrderItems = new List<OrderItem>();

            GetRandomSkus();
            GetRandomStockItems();
            GetRandomCustomers();
            GetRandomOrderPickers();
            GetRandomUsers();
            GetRandomOrders();
            GetRandomOrderItems();
        }

        private static byte[] StringToByteArray(string str)
        {
            var encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        private static Faker<Sku> SkuFaker()
        {
            return new Faker<Sku>()
            .RuleFor(s => s.Id, _ => Guid.NewGuid())
            .RuleFor(s => s.Name, f => f.Commerce.ProductName())
            .RuleFor(s => s.Description, f => f.Commerce.ProductDescription())
            .RuleFor(s => s.Price, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(s => s.Photo, f => StringToByteArray(
                f.Image.LoremFlickrUrl(keywords: "food")));
        }

        private static void GetRandomSkus()
        {
            var faker = SkuFaker();
            Skus.AddRange(faker.Generate(_numSkus));
        }

        private static Faker<StockItem> StockItemFaker(Guid skuId)
        {
            return new Faker<StockItem>()
            .RuleFor(si => si.StockBalance, f => f.Random.UShort(1, 300))
            .RuleFor(si => si.Id, Guid.NewGuid())
            .RuleFor(si => si.SkuId, _ => skuId);
        }

        private static void GetRandomStockItems()
        {
            foreach (var sku in Skus)
            {
                var faker = StockItemFaker(sku.Id);
                StockItems.Add(faker.Generate());
            }
        }

        private static Faker<OrderItem> OrderItemFaker()
        {
            return new Faker<OrderItem>()
                .RuleFor(oi => oi.Id, _ => Guid.NewGuid())
                .RuleFor(oi => oi.Quantity, f => f.Random.UInt(1, 50))
                .RuleFor(oi => oi.SkuId, f => f.PickRandom(Skus).Id)
                .RuleFor(oi => oi.OrderId, f => f.PickRandom(Orders).Id);
        }

        private static void GetRandomOrderItems()
        {
            var faker = OrderItemFaker();
            OrderItems.AddRange(faker.Generate(_numOrderItems));
        }

        private static Faker<User> UserFaker(string role, Guid? customerId, Guid? orderPickerId)
        {
            return new Faker<User>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.Role, _ => role)
            .RuleFor(u => u.Password, f => f.Internet.Password(8))
            .RuleFor(u => u.CustomerId, _ => customerId)
            .RuleFor(u => u.OrderPickerId, _ => orderPickerId);
        }

        private static void GetRandomUsers()
        {
            var customerIds = Customers.Select(c => c.Id);
            foreach (var id in customerIds)
            {
                var faker = UserFaker(UserRole.Customer, id, null);
                Users.Add(faker.Generate());
            }
            var pickerIds = OrderPickers.Select(op => op.Id);
            foreach (var id in pickerIds)
            {
                var faker = UserFaker(UserRole.Picker, null, id);
                Users.Add(faker.Generate());
            }
        }

        private static Faker<Customer> CustomerFaker()
        {
            return new Faker<Customer>()
            .RuleFor(c => c.Id, _ => Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.Address, f => f.Address.StreetAddress(false))
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber("###-###-####"));
        }

        private static void GetRandomCustomers()
        {
            var faker = CustomerFaker();
            Customers.AddRange(faker.Generate(_numCustomers));
        }

        private static Faker<OrderPicker> OrderPickerFaker()
        {
            return new Faker<OrderPicker>()
            .RuleFor(op => op.Id, _ => Guid.NewGuid())
            .RuleFor(op => op.Name, (f, op) => f.Name.FullName());
        }

        private static void GetRandomOrderPickers()
        {
            var faker = OrderPickerFaker();
            OrderPickers.AddRange(faker.Generate(_numPickers));
        }

        private static Faker<Order> OrderFaker()
        {
            return new Faker<Order>()
            .RuleFor(o => o.Id, _ => Guid.NewGuid())
            .RuleFor(o => o.OpenTime, f => f.Date.Past())
            .RuleFor(o => o.CloseTime, f => f.Date.Recent())
            .RuleFor(o => o.OrderStatus, _ => OrderStatus.Finished)
            .RuleFor(o => o.CustomerId, f => f.PickRandom(Customers).Id)
            .RuleFor(o => o.OrderPickerId, f => f.PickRandom(OrderPickers).Id);
        }

        private static void GetRandomOrders()
        {
            var faker = OrderFaker();
            Orders.AddRange(faker.Generate(_numOrders));
        }
    }
}
