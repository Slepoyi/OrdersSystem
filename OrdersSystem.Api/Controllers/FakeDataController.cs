using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Api.Auth.Middleware;
using OrdersSystem.Data.Process.DataRefresh;

namespace OrdersSystem.Api.Controllers
{
    [Authorize]
    [Route("api/data/")]
    [ApiController]
    public class FakeDataController : ControllerBase
    {
        private readonly IDbSetsRefresher _dbSetsRefresher;

        public FakeDataController(IDbSetsRefresher dbSetsRefresher)
        {
            _dbSetsRefresher = dbSetsRefresher;
        }

        [HttpPost]
        public void RefreshDb()
        {
            _dbSetsRefresher.Refresh();
        }
    }
}
