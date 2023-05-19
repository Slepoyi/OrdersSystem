using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OpenTime { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public DateTime PickingStartTime { get; set; }
        public DateTime CloseTime { get; set; }
        [ForeignKey("OrderPickerId")]
        public virtual OrderPicker? OrderPicker { get; set; }
        public Guid OrderPickerId { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
