using Microsoft.EntityFrameworkCore;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;
using System.Reflection.Metadata;

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
            modelBuilder.Entity<StockItem>()
                .HasOne(s => s.Sku)
                .WithOne()
                .HasForeignKey("SkuId")
                .HasPrincipalKey("Id");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerGuid)
                .HasPrincipalKey(c => c.Id);

            modelBuilder.Entity<Order>().OwnsMany(o => o.OrderItems, a =>
            {
                a.WithOwner().HasForeignKey("Sku");
            });
            

            base.OnModelCreating(modelBuilder);
        }
    }
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedOn { get; set; }
        public bool Archived { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Uri SiteUri { get; set; }

        public ICollection<Post> Posts { get; }
    }
}
