using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Api.Auth.Services;
using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.Api.Controllers
{
    [ApiController]
    [Route("api/auth/")]
    [Produces("application/json")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;

        public AuthenticationController(IJwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        /// <summary>
        /// Generates a jwt token for provided loginModel
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>A newly generated jwt token</returns>
        /// <response code="200">Returns the newly generated token</response>
        /// <response code="400">If loginModel is invalid</response>
        [HttpPost("authenticate/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.GetByUsernameConsideringPasswordAsync(loginModel);

            if (user is null)
                return BadRequest("Cannot find user with these username and password");

            var token = _jwtService.GenerateToken(user);
            return Ok(token);
        }
    }
}
