using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class Order
    {
        public Order(Customer customer, DateTime openTime,
          List<OrderItem> skus) : this(openTime)
        {
            Customer = customer;
            OrderItems = skus;
        }

        private Order(DateTime openTime) : base()
        {
            Id = Guid.NewGuid();
            OpenTime = openTime;
        }

        [Key]
        public Guid Id { get; private set; }
        [Required]
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; private set; }
        public DateTime OpenTime { get; private set; }
        public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
        public DateTime PickingStartTime { get; set; }
        public DateTime CloseTime { get; set; }
        [ForeignKey("PickerId")]
        public virtual OrderPicker OrderPicker { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public decimal GetTotalPrice()
        {
            decimal totalPrice = 0;

            foreach (var item in OrderItems)
            {
                totalPrice += item.Quantity * item.Sku.Price;
            }

            return totalPrice;
        }

        public bool ReduceSku(OrderItem item, uint quantity)
        {
            var currentItem = OrderItems.FirstOrDefault(s => s.Sku == item.Sku);
            if (currentItem is null)
                return false;

            if (currentItem.Quantity <= quantity)
                OrderItems.Remove(currentItem);
            else
                currentItem.ReduceQuantity(quantity);

            return true;
        }

        public void AddSku(OrderItem item, uint quantity)
        {
            var currentItem = OrderItems.FirstOrDefault(s => s.Sku == item.Sku);

            if (currentItem is null)
                OrderItems.Add(item);
            else
                currentItem.IncreaseQuantity(quantity);
        }
    }
}
