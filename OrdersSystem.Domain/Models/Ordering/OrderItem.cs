using Microsoft.EntityFrameworkCore;
using OrdersSystem.Domain.Models.Stock;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Ordering
{
    [Owned]
    public class OrderItem
    {
        public OrderItem(Sku sku, uint quantity) : this(quantity)
        {
            Sku = sku;
        }

        private OrderItem(uint quantity) : base()
        {
            Quantity = quantity;
        }

        [Key]
        public Sku Sku { get; private set; }
        [Required]
        public uint Quantity { get; set; }
    }
}
