using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersSystem.Domain.Models.Stock
{
    public class StockItem
    {
        public StockItem(Sku sku, uint stockBalance) : this(stockBalance)
        {
            Sku = sku;
        }

        private StockItem(uint stockBalance) : base()
        {
            StockBalance = stockBalance;
        }

        [Key, ForeignKey(nameof(Sku))]
        public Guid Id { get; set; }
        public virtual Sku Sku { get; set; }
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
