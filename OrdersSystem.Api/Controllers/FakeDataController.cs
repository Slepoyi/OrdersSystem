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
        /// Refreshes all the DbSets with randomly generated information
        /// </summary>
        /// <param name=""></param>
        /// <returns>Ok</returns>
        /// <response code="200">Returns confirmation of DbSets refreshing</response>
        [HttpPost("refresh/")]
        public IActionResult RefreshDb()
        {
            _dbSetsRefresher.Refresh();
            return Ok();
        }
    }
}
