﻿using Bogus;
using Microsoft.Extensions.Options;
using OrdersSystem.Data.Process.Options;
using OrdersSystem.Domain.Enums;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;
using System.Text;

namespace OrdersSystem.Data.Process.DataRefresh
{
    public class DataGenerator : IDataGenerator
    {
        private readonly FakerOptions _fakerOptions;

        public DataGenerator(IOptions<FakerOptions> options)
        {
            _fakerOptions = options.Value;
        }

        public List<User> Users { get; private set; } = new();
        public List<Sku> Skus { get; private set; } = new();
        public List<StockItem> StockItems { get; private set; } = new();
        public List<Customer> Customers { get; private set; } = new();
        public List<OrderPicker> OrderPickers { get; private set; } = new();
        public List<OrderItem> OrderItems { get; set; } = new();
        public List<Order> Orders { get; private set; } = new();

        public void InitData()
        {
            Randomizer.Seed = new Random(42069);

            Users = new();
            Skus = new();
            StockItems = new();
            Customers = new();
            OrderPickers = new();
            OrderItems = new();

            GetRandomSkus();
            GetRandomStockItems();
            GetRandomUsers();
            GetRandomCustomers();
            GetRandomOrderPickers();
            GetRandomOrders();
            GetRandomOrderItems();
        }

        private static byte[] StringToByteArray(string str)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        private static Faker<Sku> SkuFaker()
        {
            return new Faker<Sku>()
            .RuleFor(s => s.Id, f => f.Random.Guid())
            .RuleFor(s => s.Name, f => f.Commerce.ProductName())
            .RuleFor(s => s.Description, f => f.Commerce.ProductDescription())
            .RuleFor(s => s.Price, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(s => s.Photo, f => StringToByteArray(
                f.Image.LoremFlickrUrl(keywords: "food")));
        }

        private void GetRandomSkus()
        {
            var faker = SkuFaker();
            Skus.AddRange(faker.Generate(_fakerOptions.NumSkus));
        }

        private static Faker<StockItem> StockItemFaker(Guid skuId)
        {
            return new Faker<StockItem>()
            .RuleFor(si => si.Quantity, f => f.Random.UShort(1, 300))
            .RuleFor(si => si.Id, f => f.Random.Guid())
            .RuleFor(si => si.SkuId, _ => skuId);
        }

        private void GetRandomStockItems()
        {
            foreach (var sku in Skus)
            {
                var faker = StockItemFaker(sku.Id);
                StockItems.Add(faker.Generate());
            }
        }

        private Faker<OrderItem> OrderItemFaker()
        {
            return new Faker<OrderItem>()
                .RuleFor(oi => oi.Id, f => f.Random.Guid())
                .RuleFor(oi => oi.Quantity, f => f.Random.UInt(1, 50))
                .RuleFor(oi => oi.SkuId, f => f.PickRandom(Skus).Id)
                .RuleFor(oi => oi.OrderId, f => f.PickRandom(Orders).Id);
        }

        private void GetRandomOrderItems()
        {
            var faker = OrderItemFaker();
            OrderItems.AddRange(faker.Generate(_fakerOptions.NumOrderItems));
        }

        private static Faker<User> UserFaker(string role)
        {
            return new Faker<User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Role, _ => role)
            .RuleFor(u => u.Password, f => f.Internet.Password(8));
        }

        private void GetRandomUsers()
        {
            var cFaker = UserFaker(UserRole.Customer);
            Users.AddRange(cFaker.Generate(_fakerOptions.NumCustomers));
            var opFaker = UserFaker(UserRole.Picker);
            Users.AddRange(opFaker.Generate(_fakerOptions.NumPickers));
        }

        private static Faker<Customer> CustomerFaker(Guid id)
        {
            return new Faker<Customer>()
            .RuleFor(c => c.Id, _ => id)
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.Address, f => f.Address.StreetAddress(false))
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber("###-###-####"));
        }

        private void GetRandomCustomers()
        {
            var userIds = Users.Where(u => u.Role == UserRole.Customer).Select(u => u.Id);
            foreach (var id in userIds)
            {
                var faker = CustomerFaker(id);
                Customers.Add(faker.Generate());
            }
        }

        private static Faker<OrderPicker> OrderPickerFaker(Guid id)
        {
            return new Faker<OrderPicker>()
            .RuleFor(op => op.Id, _ => id)
            .RuleFor(op => op.Name, (f, op) => f.Name.FullName());
        }

        private void GetRandomOrderPickers()
        {
            var userIds = Users.Where(u => u.Role == UserRole.Picker).Select(u => u.Id);
            foreach (var id in userIds)
            {
                var faker = OrderPickerFaker(id);
                OrderPickers.Add(faker.Generate());
            }
            OrderPickers.Add(new OrderPicker { Id = Guid.Empty, Name = "Mr Nobody" });
        }

        private Faker<Order> OrderFaker()
        {
            return new Faker<Order>()
            .RuleFor(o => o.Id, f => f.Random.Guid())
            .RuleFor(o => o.OpenTime, f => f.Date.Past())
            .RuleFor(o => o.PickingStartTime, (f, o) => f.Date.Recent(1, o.OpenTime))
            .RuleFor(o => o.CloseTime, (f, o) => f.Date.Recent(1, o.PickingStartTime))
            .RuleFor(o => o.OrderStatus, _ => OrderStatus.Finished)
            .RuleFor(o => o.CustomerId, f => f.PickRandom(Customers).Id)
            .RuleFor(o => o.OrderPickerId, f => f.PickRandom(
                OrderPickers.Where(op => op.Id != Guid.Empty)).Id);
        }

        private void GetRandomOrders()
        {
            var faker = OrderFaker();
            Orders.AddRange(faker.Generate(_fakerOptions.NumOrders));
        }
    }
}
