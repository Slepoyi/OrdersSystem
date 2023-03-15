using OrdersSystem.Domain.Models;

namespace OrdersSystem.Data.Process.Validation
{
    public class OrderValidator
    {
        public ValidationResult ValidateOrder(Dictionary<Sku, uint> order, IEnumerable<StockItem> stock)
        {
            var result = new ValidationResult();

            foreach (var orderItem in order)
            {
                var stockItem = stock.FirstOrDefault(s => s.Sku.Id == orderItem.Key.Id);
                ValidateOrderItem(result, orderItem, stockItem);
            }

            return result;
        }

        private void ValidateOrderItem(ValidationResult result, KeyValuePair<Sku, uint> orderItem, StockItem? stockItem)
        {
            if (stockItem is null)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Sku '{orderItem.Key.Name}' not found in stock.");
                result.ErrorCodes.Add(OrderErrorCode.SkuIdNotFound);
            }
            else if (stockItem.StockBalance < orderItem.Value)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Insufficient stock for sku '{orderItem.Key.Name}'.");
                result.ErrorCodes.Add(OrderErrorCode.InsufficientSkuStock);
            }
            else if (stockItem.Sku.Price != orderItem.Key.Price)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Wrong price {orderItem.Key.Price} for '{orderItem.Key.Name}'.");
                result.ErrorCodes.Add(OrderErrorCode.WrongSkuPrice);
            }
        }
    }
}
