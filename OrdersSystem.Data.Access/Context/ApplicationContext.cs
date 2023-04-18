using Microsoft.EntityFrameworkCore;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Access.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderPicker> OrderPickers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<StockItem> ReservedItems { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockItem>().HasForeignKey(s => s.);

            modelBuilder.Entity<Order>().OwnsMany(o => o.OrderItems, a =>
            {
                a.WithOwner().HasForeignKey("Sku");
            });
            

            base.OnModelCreating(modelBuilder);
        }
    }
}
