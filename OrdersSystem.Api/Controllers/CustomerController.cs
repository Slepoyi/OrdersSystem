using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Api.Auth.Middleware;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Domain.Enums;
using OrdersSystem.Domain.Models.Auth;
using OrdersSystem.Domain.Models.Extensions;
using OrdersSystem.Domain.Models.Ordering;
using OrdersSystem.Domain.Models.Stock;
using System.Diagnostics;

namespace OrdersSystem.Api.Controllers
{
    [ApiController]
    [Route("api/customers/")]
    [Produces("application/json")]
    [Authorize(UserRole.Customer)]
    public class CustomerController : ControllerBase
    {
        private readonly IOrderFlowManager _orderManager;

        public CustomerController(IOrderFlowManager orderManager)
        {
            _orderManager = orderManager;
        }

        /// <summary>
        /// Creates an Order
        /// </summary>
        /// <param name="orderItems"></param>
        /// <returns>A newly generated order</returns>
        /// <response code="201">Returns the newly created order</response>
        /// <response code="400">If any of StockItems is invalid</response>
        [HttpPost("create_order/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrderAsync([FromBody] IEnumerable<OrderItem> orderItems)
        {
            var user = HttpContext.Items["User"] as User;

            var stockItems = _orderManager.GetStockForOrderItems(orderItems);

            var validationResult = _orderManager.CustomerValidateOrder(orderItems, stockItems);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.ErrorMessages);

            var order = await _orderManager.CreateOrderAsync(orderItems, user.Id, stockItems);
            if (order is null)
                return BadRequest();

            return CreatedAtAction(nameof(GetByGuidAsync), new { id = order.Id }, order.ToOrderDto());
        }

        /// <summary>
        /// Cancels an Order
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Confirmation that order was cancelled</returns>
        /// <response code="200">Order was cancelled</response>
        /// <response code="403">Order does not belong to the customer</response>
        /// <response code="404">Order with such an id does not exist</response>
        /// <response code="501">Order was processed and cannot be cancelled</response>
        [HttpPost("cancel_order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public async Task<IActionResult> CancelOrderAsync(Guid id)
        {
            var user = HttpContext.Items["User"] as User;

            var order = await _orderManager.GetByGuidAsync(id);

            if (order is null)
                return NotFound(new { Detail = "Order with this id was not found" });

            if (order.CustomerId != user.Id)
                return Problem(detail: "You cannot track this order.", statusCode: StatusCodes.Status403Forbidden);

            var cancelled = await _orderManager.CancelOrderAsync(order);

            if (!cancelled)
                return Problem(detail: "Sorry, your order cannot be cancelled.", statusCode: StatusCodes.Status501NotImplemented);

            return Ok(new { Detail = "Order was cancelled" });
        }

        /// <summary>
        /// Retrieve information about an Order
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Order entity</returns>
        /// <response code="200">Returns an Order entity</response>
        /// <response code="403">Order does not belong to the customer</response>
        /// <response code="404">Order with such an id does not exist</response>
        [HttpGet("get_order/{id}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByGuidAsync(Guid id)
        {
            var user = HttpContext.Items["User"] as User;

            var order = await _orderManager.GetByGuidAsync(id);

            if (order is null)
                return NotFound();

            if (order.CustomerId != user.Id)
                return Problem(detail: "You cannot track this order.", statusCode: StatusCodes.Status403Forbidden);

            return Ok(order.ToOrderDto());
        }

        /// <summary>
        /// Retrieve information about the stock
        /// </summary>
        /// <returns>Collection of StockItems</returns>
        /// <response code="200">Returns a collection of Stockitems</response>
        [HttpGet("get_stock/")]
        [ProducesResponseType(typeof(IEnumerable<StockItem>), StatusCodes.Status200OK)]
        public IActionResult GetStock()
        {
            return Ok(_orderManager.GetStock());
        }
    }
}
