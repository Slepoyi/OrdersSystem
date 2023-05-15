using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Data.Process.DataRefresh;

namespace OrdersSystem.Api.Controllers
{
    [Route("api/data/")]
    [ApiController]
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
        /// <returns></returns>
        [HttpPost("refresh/")]
        public void RefreshDb()
        {
            _dbSetsRefresher.Refresh();
        }
    }
}
