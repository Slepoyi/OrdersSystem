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

        public void ReduceBalance(uint amount)
        {
            if (StockBalance < amount) { }

            StockBalance -= amount;
        }

        public void IncreaseBalance(uint amount)
        {
            StockBalance += amount;
        }
    }
}
