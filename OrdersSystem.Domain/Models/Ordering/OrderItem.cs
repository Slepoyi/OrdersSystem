using OrdersSystem.Domain.Models.Stock;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class OrderItem
    {
        public OrderItem(Sku sku, uint quantity)
        {
            Sku = sku;
            Quantity = quantity;
        }

        [Required]
        public Sku Sku { get; private set; }
        [Required]
        public uint Quantity { get; set; }
    }
}
