using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderPickerController : ControllerBase
    {
        [HttpPost("{id}")]
        public async Task<IActionResult> CloseOrderAsync(Guid id, [FromBody]IEnumerable<OrderSku> orderSku)
        {
            var closed = _orderManager.CloseOrder(id, orderSku, User.Identity.Name);
            if (!closed)
                return SorryThereIsSomethingWrongWithRequestParams; // 400 with custom code
            // close order, assign ordersku to order skus list and change status of an order
            // find difference between final order and reserve and rollback stock of the difference
        }
        [HttpPost]
        public async Task<IActionResult> GetNextOrderAsync()
        {
            var order = _orderQueue.GetNextOrderAsync();
            if (order is null)
                return SorryNoOrderForYouRightNow(); // 404 with custom message

            _orderManager.BeginOrderPickingAsync(order, User.Identity?.Name);

            return Ok(order);
            // get next order from order queue
            // picker begins picking an order
            // set order pickingstarted time, picker and status
            // is it post or get really?
        }
    }
}
