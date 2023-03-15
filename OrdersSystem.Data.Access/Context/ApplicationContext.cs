using Microsoft.EntityFrameworkCore;
using OrdersSystem.Domain.Models.Actors;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Access.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderPicker> OrderPickers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Sku> Skus { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
    }
}
