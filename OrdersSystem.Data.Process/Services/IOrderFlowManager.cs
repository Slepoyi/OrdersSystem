﻿using OrdersSystem.Data.Process.Validation;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;

namespace OrdersSystem.Data.Process.Services
{
    public interface IOrderFlowManager
    {
        ValidationResult ValidateOrder(IEnumerable<OrderItem> orderItems, IEnumerable<StockItem> stockItems);
        IEnumerable<StockItem> GetStockForOrderItems(IEnumerable<OrderItem> orderItems);
        Task<Order?> GetByGuidAsync(Guid id);
        Task<Order?> CreateOrderAsync(IEnumerable<OrderItem> orderItems, Guid userGuid, IEnumerable<StockItem> stockItems);
        Task<bool> CancelOrderAsync(Order order);
    }
}