using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Api.Auth.Middleware;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Domain.Enums;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Ordering;

namespace OrdersSystem.Api.Controllers
{
    [ApiController]
    [Route("api/pickers/")]
    [Produces("application/json")]
    [Authorize(UserRole.Picker)]
    public class OrderPickerController : ControllerBase
    {
        private readonly IOrderFlowManager _orderManager;

        public OrderPickerController(IOrderFlowManager orderManager)
        {
            _orderManager = orderManager;
        }

        /// <summary>
        /// Confirms that order picking has started
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Confirmation that order was succesfully updated</returns>
        /// <response code="200">Order was succesfully updated</response>
        /// <response code="400">Order data is invalid</response>
        [HttpPost("confirm/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FinishOrder([FromBody] Order order)
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
        }

        /// <summary>
        /// Assign an order to the OrderPicker
        /// </summary>
        /// <param name=""></param>
        /// <returns>Confirmation that order was assigned</returns>
        /// <response code="200">Order was assigned to the OrderPicker</response>\
        /// <response code="404">There is no any Order which can be assigned</response>
        /// <response code="500">Internal error finding the OrderPicker in the database</response>
        [HttpPost("assign/")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
