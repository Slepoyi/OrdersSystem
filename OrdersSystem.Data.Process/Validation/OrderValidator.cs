using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.Validation
{
    public class OrderValidator : IOrderValidator
    {
        public ValidationResult ValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<StockItem> stock)
        {
            var result = new ValidationResult();

            IEnumerable<Guid> duplicateIds = orderItems
                .GroupBy(i => i.Id)
                .Where(i => i.Count() > 1)
                .Select(i => i.Key);

            if (duplicateIds.Any())
            {
                result.IsValid = false;
                result.ErrorCodes.Add(OrderErrorCode.DuplicateId);
                foreach (var duplicateId in duplicateIds)
                    result.ErrorMessages.Add($"Duplicate OrderItem Id {duplicateId} was found.");
            }

            foreach (var orderItem in orderItems)
            {
                var stockItem = stock.FirstOrDefault(s => s.SkuId == orderItem.SkuId);
                ValidateOrderItem(result, orderItem, stockItem);
            }

            return result;
        }

        private void ValidateOrderItem(ValidationResult result, OrderItem orderItem, StockItem? stockItem)
        {
            if (stockItem is null)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Sku '{orderItem.SkuId}' not found in stock.");
                result.ErrorCodes.Add(OrderErrorCode.SkuIdNotFound);
            }
            else if (stockItem.Quantity < orderItem.Quantity)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Insufficient stock for sku '{orderItem.Sku.Name}'.");
                result.ErrorCodes.Add(OrderErrorCode.InsufficientSkuStock);
            }
        }
    }
}
