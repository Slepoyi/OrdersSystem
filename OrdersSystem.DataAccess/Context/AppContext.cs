using OrdersSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersSystem.DataAccess.Context
{
    public class AppContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderPicker> OrderPickers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Sku> Skus { get; set; }
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {

        }
    }
}
