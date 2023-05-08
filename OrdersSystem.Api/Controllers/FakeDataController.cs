using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Api.Auth.Middleware;
using OrdersSystem.Data.Refresh;

namespace OrdersSystem.Api.Controllers
{
    //[Authorize]
    [Route("api/data/")]
    [ApiController]
    public class FakeDataController : ControllerBase
    {
        private readonly IRefreshDbSets _refreshDbSet;

        public FakeDataController(IRefreshDbSets refreshDbSet)
        {
            _refreshDbSet = refreshDbSet;
        }

        [HttpPost]
        public async Task RefreshDb()
        {
            _refreshDbSet.Refresh();
        }
    }
}
