using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Api.Auth.Middleware;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Domain.Enums;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;

namespace OrdersSystem.Api.Controllers
{
    [Authorize(UserRole.Customer)]
    [Route("api/customers/")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IOrderFlowManager _orderManager;

        // create user order
        // all skus are reserved in database
        // return guid of order or error
        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] IEnumerable<OrderItem> orderItems)
        {
            if (HttpContext.Items["User"] is not User user)
                return Problem("Error creating user identity.");

            var stockItems = _orderManager.GetStockForOrderItems(orderItems);

            var validationResult = _orderManager.ValidateOrder(orderItems, stockItems);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.ErrorMessages);

            var order = await _orderManager.CreateOrderAsync(orderItems, user.Id, stockItems);
            if (order is null)
                return BadRequest();

            return CreatedAtAction(nameof(GetByGuidAsync), new { id = order.Id }, order);
        }

        // cancel order if possible
        // rollback of reserve
        [HttpPost("{id}")]
        public async Task<IActionResult> CancelOrderAsync(Guid id)
        {
            if (HttpContext.Items["User"] is not User user)
                return Problem("Error creating user identity.");

            var order = await _orderManager.GetByGuidAsync(id);

            if (order is null)
                return NotFound();

            if (order.Customer.Id != user.Id)
                return Forbid("You cannot cancel this order.");

            var cancelled = await _orderManager.CancelOrderAsync(order);

            if (!cancelled)
                return Problem(detail: "Sorry, your order cannot be cancelled.", statusCode: 501);

            return Ok("Order was cancelled");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByGuidAsync(Guid id)
        {
            if (HttpContext.Items["User"] is not User user)
                return Problem("Error creating user identity.");

            var order = await _orderManager.GetByGuidAsync(id);

            if (order is null)
                return NotFound();

            if (order.Customer.Id != user.Id)
                return Forbid("You cannot track this order.");

            return Ok(order);
        }
    }
}
