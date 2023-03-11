using System.ComponentModel.DataAnnotations;

namespace OrdersSystem.Domain.Models
{
    public class Order
    {
        public Order(Customer customer, DateTime openTime,
           List<OrderSku> skus)
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
        public DateTime PickingStartTime { get; private set; }
        public DateTime CloseTime { get; private set; }
        public List<OrderSku> Skus { get; private set; }
        public OrderPicker? OrderPicker { get; private set; }
        public OrderStatus OrderStatus { get; private set; }

       

        public void ProcessOrder(DateTime pickingStartTime, OrderPicker orderPicker)
        {
            if (OrderStatus != OrderStatus.Created)
                throw new NotImplementedException("Order is either closed or already being processed");

            OrderPicker = orderPicker;
            OrderStatus = OrderStatus.Processing;
            PickingStartTime = pickingStartTime;
        }

        public void ReduceSkuPosition(OrderSku sku)
        {
            var skuInOrder = Skus.Where(s => s.Id == sku.Id).FirstOrDefault();
            if (skuInOrder is null)
                throw new NotImplementedException(
                    $"Cannot remove {sku.Name} from the order because it is not presented in the order");

            var remainder = skuInOrder.Amount - sku.Amount;

            if (remainder > 0)
                skuInOrder.SetAmount(remainder);
            if (remainder == 0)
                Skus.Remove(skuInOrder);
            if (remainder < 0)
                throw new NotImplementedException(
                    $"Cannot reduce {sku.Name} from {skuInOrder.Amount} to {sku.Amount}");
        }
    }
}
