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
        public uint StockBalance { get; set; }

        public void ReduceBalance(uint quantity)
        {
            if (StockBalance < quantity) { }

            StockBalance -= quantity;
        }

        public void IncreaseBalance(uint quantity)
        {
            StockBalance += quantity;
        }
    }
}
