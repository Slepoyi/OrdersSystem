using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.Validation
{
    public class OrderValidator : IOrderValidator
    {
        public ValidationResult ValidateOrder(IEnumerable<OrderItem> order, IEnumerable<StockItem> stock)
        {
            var result = new ValidationResult();

            foreach (var orderItem in order)
            {
                var stockItem = stock.FirstOrDefault(s => s.Sku.Id == orderItem.Sku.Id);
                ValidateOrderItem(result, orderItem, stockItem);
            }

            return result;
        }

        private void ValidateOrderItem(ValidationResult result, OrderItem orderItem, StockItem? stockItem)
        {
            if (stockItem is null)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Sku '{orderItem.Sku.Name}' not found in stock.");
                result.ErrorCodes.Add(OrderErrorCode.SkuIdNotFound);
            }
            else if (stockItem.StockBalance < orderItem.Quantity)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Insufficient stock for sku '{orderItem.Sku.Name}'.");
                result.ErrorCodes.Add(OrderErrorCode.InsufficientSkuStock);
            }
            else if (stockItem.Sku.Price != orderItem.Sku.Price)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Wrong price {orderItem.Sku.Price} for '{orderItem.Sku.Name}'.");
                result.ErrorCodes.Add(OrderErrorCode.WrongSkuPrice);
            }
        }
    }
}
