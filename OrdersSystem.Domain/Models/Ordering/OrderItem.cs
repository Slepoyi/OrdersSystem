using OrdersSystem.Domain.Models.Stock;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersSystem.Domain.Models.Ordering
{
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
        public Guid Id { get; set; }

        [ForeignKey("SkuId")]
        public virtual Sku Sku { get; set; }

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
