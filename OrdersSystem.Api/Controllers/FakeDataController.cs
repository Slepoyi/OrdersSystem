 using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Data.Process.DataRefresh;

namespace OrdersSystem.Api.Controllers
{
    [ApiController]
    [Route("api/data/")]
    public class FakeDataController : ControllerBase
    {
        private readonly IDbSetsRefresher _dbSetsRefresher;

        public FakeDataController(IDbSetsRefresher dbSetsRefresher)
        {
            _dbSetsRefresher = dbSetsRefresher;
        }

        /// <summary>
        /// Refreshes all the DbSets with newly generated information
        /// </summary>
        /// <returns>Ok</returns>
        /// <response code="200">Returns confirmation of DbSets refreshing</response>
        [HttpPost("_refresh_customer/")]
        public IActionResult RefreshDbCustomer()
        {
            _dbSetsRefresher.RefreshForCustomerTests();
            return Ok();
        }

        /// <summary>
        /// Refreshes all the DbSets with newly generated information
        /// </summary>
        /// <returns>Ok</returns>
        /// <response code="200">Returns confirmation of DbSets refreshing</response>
        [HttpPost("_refresh_picker/")]
        public IActionResult RefreshDbPicker()
        {
            _dbSetsRefresher.RefreshForPickerTests();
            return Ok();
        }
    }
}
