using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersSystem.Domain.Models.Stock
{
    public class StockItem
    {
        public StockItem() { }
        public StockItem(Sku sku, Guid id, uint stockBalance) : this(id, stockBalance)
        {
            Sku = sku;
        }

        private StockItem(Guid id, uint stockBalance) : base()
        {
            Id = id;
            StockBalance = stockBalance;
        }
        [Key]
        public Guid Id { get; set; }
        public virtual Sku Sku { get; set; }
        public Guid SkuId { get; set; }
        [Required]
        public uint StockBalance { get; private set; }

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
