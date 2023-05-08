using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Api.Auth.Middleware;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Domain.Enums;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;

namespace OrdersSystem.Api.Controllers
{
    [Authorize(UserRole.Picker)]
    [Route("api/pickers/")]
    [ApiController]
    public class OrderPickerController : ControllerBase
    {
        private readonly IOrderFlowManager _orderManager;

        public OrderPickerController(IOrderFlowManager orderManager)
        {
            _orderManager = orderManager;
        }

        [HttpPost("confirm/")]
        public async Task<IActionResult> ConfirmOrderPicking([FromBody] Order order)
        {
            var user = HttpContext.Items["User"] as User;

            var stockItems = _orderManager.GetStockForOrderItems(order.OrderItems);

            var validationResult = _orderManager.ValidateOrder(order.OrderItems, stockItems);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.ErrorMessages);

            var closed = await _orderManager.CloseOrder(order, user.Id);

            if (!closed)
                return BadRequest("Sorry, there is something wrong with your order data.");

            return Ok("Order was successfully closed.");
            // close order, assign ordersku to order skus list and change status of an order
            // find difference between final order and reserve and rollback stock of the difference
        }

        [HttpPost("assign/")]
        public async Task<IActionResult> AssignOrderAsync()
        {
            var user = HttpContext.Items["User"] as User;

            var order = await _orderManager.GetNextOrderAsync();
            if (order is null)
                return NotFound("Sorry, there is no order for you right now.");

            if (!await _orderManager.BeginOrderPickingAsync(order, user.Id))
                return Problem();

            return Ok(order);
        }
    }
}
