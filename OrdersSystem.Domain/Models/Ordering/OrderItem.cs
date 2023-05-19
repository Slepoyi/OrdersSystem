using OrdersSystem.Domain.Models.Stock;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("SkuId")]
        public virtual Sku? Sku { get; set; }
        public Guid SkuId { get; set; }

        [Required]
        public uint Quantity { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
        public Guid OrderId { get; set; }
    }
}
