using OrdersSystem.Domain.Models.Stock;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class OrderItem
    {
        public OrderItem(Sku sku, uint amount)
        {
            Sku = sku;
            Amount = amount;
        }

        [Required]
        public Sku Sku { get; private set; }
        [Required]
        public uint Amount { get; private set; }
    }
}
