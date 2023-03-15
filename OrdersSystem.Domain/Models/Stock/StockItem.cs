using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Stock
{
    public class StockItem
    {
        public StockItem(Sku sku, uint stockBalance)
        {
            Sku = sku;
            StockBalance = stockBalance;
        }

        [Required]
        public Sku Sku { get; private set; }
        [Required]
        public uint StockBalance { get; private set; }
    }
}
