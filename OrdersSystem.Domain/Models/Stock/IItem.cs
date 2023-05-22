namespace OrdersSystem.Domain.Models.Stock
{
    public interface IItem
    {
        Guid SkuId { get; set; }
        uint Quantity { get; set; }
    }
}
