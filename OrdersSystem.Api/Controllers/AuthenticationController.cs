using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Api.Auth.Services;
using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.Api.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        public AuthenticationController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("authenticate/")]
        public IActionResult Authenticate([FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = _jwtService.GenerateToken(model);
            return Ok(token);
        }
    }
}
