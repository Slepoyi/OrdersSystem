using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class Order
    {
        public Order(Customer customer, DateTime openTime,
          List<OrderItem> skus)
        {
            Id = Guid.NewGuid();
            Customer = customer;
            OpenTime = openTime;
            OrderItems = skus;
        }

        [Key]
        public Guid Id { get; private set; }
        [Required]
        public Customer Customer { get; private set; }
        public DateTime OpenTime { get; private set; }
        public List<OrderItem> OrderItems { get; private set; }
        public DateTime PickingStartTime { get; set; }
        public DateTime CloseTime { get; set; }
        public OrderPicker? OrderPicker { get; set; }
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
                currentItem.Quantity -= quantity;

            return true;
        }

        public void AddSku(OrderItem item, uint quantity)
        {
            var currentItem = OrderItems.FirstOrDefault(s => s.Sku == item.Sku);

            if (currentItem is null)
                OrderItems.Add(item);
            else
                currentItem.Quantity += quantity;
        }
    }
}
