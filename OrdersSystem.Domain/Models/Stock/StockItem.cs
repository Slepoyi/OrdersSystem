using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersSystem.Domain.Models.Stock
{
    public class StockItem
    {
        [Key]
        public Guid Id { get; set; }
        public virtual Sku Sku { get; set; }
        public Guid SkuId { get; set; }
        [Required]
        public uint Quantity { get; set; }

        public void ReduceBalance(uint quantity)
        {
            if (Quantity < quantity) { }

            Quantity -= quantity;
        }

        public void IncreaseBalance(uint quantity)
        {
            Quantity += quantity;
        }
    }
}
