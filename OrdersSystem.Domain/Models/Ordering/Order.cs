using OrdersSystem.Domain.Models.Actors;
using OrdersSystem.Domain.Models.Stock;
using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models.Ordering
{
    public class Order
    {
        public Order(Customer customer, DateTime openTime,
          Dictionary<Sku, int> skus)
        {
            Id = Guid.NewGuid();
            Customer = customer;
            OpenTime = openTime;
            Skus = skus;
        }

        [Key]
        public Guid Id { get; private set; }
        [Required]
        public Customer Customer { get; private set; }
        public DateTime OpenTime { get; private set; }
        public Dictionary<Sku, int> Skus { get; private set; }
        public DateTime PickingStartTime { get; set; }
        public DateTime CloseTime { get; set; }
        public OrderPicker? OrderPicker { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public decimal GetTotalPrice()
        {
            decimal totalPrice = 0;

            foreach (KeyValuePair<Sku, int> skuQuantityPair in Skus)
            {
                var sku = skuQuantityPair.Key;
                var quantity = skuQuantityPair.Value;
                totalPrice += sku.Price * quantity;
            }

            return totalPrice;
        }

        public bool RemoveSku(Sku sku, int quantity)
        {
            if (!Skus.ContainsKey(sku))
                return false;

            var currentQuantity = Skus[sku];
            if (currentQuantity <= quantity)
            {
                Skus.Remove(sku);
            }
            else
            {
                Skus[sku] = currentQuantity - quantity;
            }
            return true;
        }

        public void AddSku(Sku sku, int quantity)
        {
            if (Skus.ContainsKey(sku))
            {
                Skus[sku] += quantity;
            }
            else
            {
                Skus[sku] = quantity;
            }
        }
    }
}
