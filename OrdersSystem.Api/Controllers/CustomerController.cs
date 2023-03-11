using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Domain.Models;

namespace OrdersSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] IEnumerable<OrderSku> orderSku)
        {
            if (!_orderValidator.Validate(orderSku))
                return ValidationProblemsList; // 400 with custom message

            var guid = _orderManager.CreateOrderAsync(orderSku);
            return Ok(guid);
            // create user order here
            // all skus are reserved in database
            // return guid of order or error
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> CancelOrderAsync(Guid id)
        {
            var cancelled = _orderManager.CancelOrderAsync(id);

            if (!cancelled)
                return SorryOrderCannotBeCancelledMessage; // 501 (or 409) with custom message

            return Ok("Order was cancelled");
            // cancel order if possible
            // rollback of reserve
            // should it be post?
        }
    }
}
