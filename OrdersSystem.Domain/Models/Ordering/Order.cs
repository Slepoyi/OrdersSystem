﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OpenTime { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public DateTime PickingStartTime { get; set; }
        public DateTime CloseTime { get; set; }
        [ForeignKey("OrderPickerId")]
        public virtual OrderPicker OrderPicker { get; set; }
        public Guid OrderPickerId { get; set; }
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
