﻿using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Stock
{
    public class ReserveItem
    {
        public ReserveItem() { }
        public ReserveItem(Sku sku, Guid id, uint stockBalance) : this(id, stockBalance)
        {
            Sku = sku;
        }

        private ReserveItem(Guid id, uint stockBalance) : base()
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
