using OrdersSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersSystem.Data.Process.Services
{
    public class OrderProcessor
    {
        public void CloseOrder(DateTime closeTime, Order order)
        {
            if (order.OrderStatus == OrderStatus.Finished)
                throw new NotImplementedException("Order is already closed");

            order.OrderStatus = OrderStatus.Finished;
            order.CloseTime = closeTime;
            order.Skus = 
        }

        public void ReduceSkuPosition(OrderSku sku, Order order)
        {
            var skuInOrder = order.Skus.Where(s => s.Id == sku.Id).FirstOrDefault();
            if (skuInOrder is null)
                throw new NotImplementedException(
                    $"Cannot remove {sku.Name} from the order because it is not presented in the order");

            var remainder = skuInOrder.Amount - sku.Amount;

            if (remainder > 0)
                skuInOrder.SetAmount(remainder);
            if (remainder == 0)
                order.Skus.Remove(skuInOrder);
            if (remainder < 0)
                throw new NotImplementedException(
                    $"Cannot reduce {sku.Name} from {skuInOrder.Amount} to {sku.Amount}");
        }
    }
}
