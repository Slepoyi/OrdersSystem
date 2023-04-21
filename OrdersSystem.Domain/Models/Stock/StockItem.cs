using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Stock
{
    public class StockItem
    {
        public StockItem(Sku sku, uint stockBalance, Guid skuId) : this(stockBalance)
        {
            Sku = sku;
            SkuId = skuId;
        }

        private StockItem(uint stockBalance) : base()
        {
            StockBalance = stockBalance;
        }

        public Sku Sku { get; private set; }
        [Key]
        public Guid SkuId { get; private set; }
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
