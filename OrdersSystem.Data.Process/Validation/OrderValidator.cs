using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.Validation
{
    public class OrderValidator : IOrderValidator
    {
        public ValidationResult ValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<IItem> items)
        {
            var result = new ValidationResult();

            IEnumerable<Guid> duplicateSkuIds = orderItems
                .GroupBy(i => i.SkuId)
                .Where(i => i.Count() > 1)
                .Select(i => i.Key);

            if (duplicateSkuIds.Any())
            {
                result.ErrorCodes.Add(OrderErrorCode.DuplicateId);
                foreach (var duplicateId in duplicateSkuIds)
                    result.ErrorMessages.Add($"Duplicate OrderItem SkuId {duplicateId} was found.");
            }

            foreach (var orderItem in orderItems)
            {
                var stockItem = items.FirstOrDefault(s => s.SkuId == orderItem.SkuId);
                ValidateOrderItem(result, orderItem, stockItem);
            }

            return result;
        }

        private static void ValidateOrderItem(ValidationResult result, OrderItem orderItem, IItem? stockItem)
        {
            if (ValidateZeroQuantity(result, orderItem))
                return;
            if (ValidateNullRemainings(result, orderItem, stockItem))
                return;
            if (ValidateQuantityOnStock(result, orderItem, stockItem))
                return;
        }

        private static bool ValidateNullRemainings(ValidationResult result, OrderItem orderItem, IItem? item)
        {
            if (item is null)
            {
                result.ErrorMessages.Add($"Sku '{orderItem.SkuId}' not found in stock.");
                result.ErrorCodes.Add(OrderErrorCode.SkuIdNotFound);
                return true;
            }
            return false;
        }

        private static bool ValidateZeroQuantity(ValidationResult result, OrderItem orderItem)
        {
            if (orderItem.Quantity == 0)
            {
                result.ErrorMessages.Add($"Zero quantity for '{orderItem.SkuId}'.");
                result.ErrorCodes.Add(OrderErrorCode.ZeroQuantity);
                return true;
            }
            return false;
        }

        private static bool ValidateQuantityOnStock(ValidationResult result, OrderItem orderItem, IItem item) 
        {
            if (item.Quantity < orderItem.Quantity)
            {
                result.ErrorMessages.Add($"Insufficient remaining for {item.GetType()} '{orderItem.SkuId}'.");
                result.ErrorCodes.Add(OrderErrorCode.InsufficientSkuStock);
                return true;
            }
            return false;
        }
    }
}
