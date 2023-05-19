using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Stock
{
    public class ReserveItem
    {
        [Key]
        public Guid Id { get; set; }
        public virtual Sku Sku { get; set; }
        public Guid SkuId { get; set; }
        public uint StockBalance { get; set; }
    }
}
