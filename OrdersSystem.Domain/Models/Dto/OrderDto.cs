using OrdersSystem.Domain.Models.Ordering;

namespace OrdersSystem.Domain.Models.Dto
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime OpenTime { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public DateTime PickingStartTime { get; set; }
        public DateTime CloseTime { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
