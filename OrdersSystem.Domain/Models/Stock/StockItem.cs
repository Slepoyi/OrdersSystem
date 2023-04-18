using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

        [Key]
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
