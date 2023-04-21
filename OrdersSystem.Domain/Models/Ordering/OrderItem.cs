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

        public Guid SkuId { get; private set; }
        public Sku Sku { get; private set; }
        [Required]
        public uint Quantity { get; private set; }

        public void ReduceQuantity(uint quantity)
        {
            if (Quantity < quantity) { }

            Quantity -= quantity;
        }

        public void IncreaseQuantity(uint quantity)
        {
            Quantity += quantity;
        }
    }
}
