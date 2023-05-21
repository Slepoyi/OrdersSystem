using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.Validation
{
    public class OrderValidator : IOrderValidator
    {
        public ValidationResult CustomerValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<StockItem> stock)
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
                CustomerValidateOrderItem(result, orderItem, stockItem);
            }

            return result;
        }

        private void CustomerValidateOrderItem(ValidationResult result, OrderItem orderItem, StockItem? stockItem)
        {
            if (ValidateZeroQuantity(result, orderItem))
                return;
            if (ValidateNullRemainings(result, orderItem, stockItem))
                return;
            if (ValidateQuantityInStock(result, orderItem, stockItem))
                return;
        }

        public ValidationResult PickerValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<ReserveItem> reserveItems)
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
                var reserveItem = reserveItems.FirstOrDefault(s => s.SkuId == orderItem.SkuId);
                PickerValidateOrderItem(result, orderItem, reserveItem);
            }

            return result;
        }

        private void PickerValidateOrderItem(ValidationResult result, OrderItem orderItem, ReserveItem? stockItem)
        {
            if (ValidateZeroQuantity(result, orderItem))
                return;
            if (ValidateNullRemainings(result, orderItem, stockItem))
                return;
            if (ValidateQuantityInReserve(result, orderItem, stockItem))
                return;
        }

        private bool ValidateNullRemainings<T>(ValidationResult result, OrderItem orderItem, T? item)
        {
            if (item is null)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Sku '{orderItem.SkuId}' not found in stock.");
                result.ErrorCodes.Add(OrderErrorCode.SkuIdNotFound);
                return true;
            }
            return false;
        }

        private bool ValidateZeroQuantity(ValidationResult result, OrderItem orderItem)
        {
            if (orderItem.Quantity == 0)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Zero quantity for '{orderItem.SkuId}'.");
                result.ErrorCodes.Add(OrderErrorCode.ZeroQuantity);
                return true;
            }
            return false;
        }

        private bool ValidateQuantityInStock(ValidationResult result, OrderItem orderItem, StockItem stockItem)
        {
            if (stockItem.Quantity < orderItem.Quantity)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Insufficient stock for sku '{orderItem.SkuId}'.");
                result.ErrorCodes.Add(OrderErrorCode.InsufficientSkuStock);
                return true;
            }
            return false;
        }

        private bool ValidateQuantityInReserve(ValidationResult result, OrderItem orderItem, ReserveItem reserveItem)
        {
            if (reserveItem.StockBalance < orderItem.Quantity)
            {
                result.IsValid = false;
                result.ErrorMessages.Add($"Insufficient reserve for sku '{orderItem.SkuId}'.");
                result.ErrorCodes.Add(OrderErrorCode.InsufficientSkuStock);
                return true;
            }
            return false;
        }
    }
}
