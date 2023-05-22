using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Api.Auth.Middleware;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Domain.Enums;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Extensions;
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
        /// <param name="orderItems"></param>
        /// <param name="id"></param>
        /// <returns>Confirmation that order was succesfully updated</returns>
        /// <response code="200">Order was succesfully updated</response>
        /// <response code="400">Order data is invalid</response>
        /// <response code="403">Order does not belong to the picker</response>
        [HttpPost("finish/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> FinishOrder([FromBody] IEnumerable<OrderItem> orderItems, Guid id)
        {
            var user = HttpContext.Items["User"] as User;

            var reserveItems = _orderManager.GetReserveForOrderItems(orderItems);

            var validationResult = _orderManager.ValidateOrder(orderItems, reserveItems);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.ErrorMessages);

            var order = await _orderManager.GetByGuidAsync(id);
            if (order is null)
                return NotFound(new { Detail = "There is no order with this id." });

            if (order.OrderPickerId != user.Id)
                return Problem(detail: "You cannot finish this order.", statusCode: StatusCodes.Status403Forbidden);

            var closed = await _orderManager.CloseOrder(order, user.Id, orderItems);
            if (!closed)
                return BadRequest(new { Detail = "Sorry, there is something wrong with your order data." });

            return Ok(new { Detail = "Order was successfully closed." });
        }

        /// <summary>
        /// Assign an order to the OrderPicker
        /// </summary>
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
                return NotFound(new { Detail = "Sorry, there is no order for you right now." });

            if (!await _orderManager.BeginOrderPickingAsync(order, user.Id))
                return Problem();

            return Ok(order.ToOrderDto());
        }
    }
}
