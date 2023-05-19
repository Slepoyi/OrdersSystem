using OrdersSystem.Domain.Models.Dto;
using OrdersSystem.Domain.Models.Ordering;

namespace OrdersSystem.Domain.Models.Extensions
{
    public static class ModelsExtensions
    {
        public static OrderDto ToOrderDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderItems = order.OrderItems,
                OrderStatus = order.OrderStatus,
                OpenTime = order.OpenTime,
                CloseTime = order.CloseTime,
                PickingStartTime = order.PickingStartTime
            };
        }
    }
}
