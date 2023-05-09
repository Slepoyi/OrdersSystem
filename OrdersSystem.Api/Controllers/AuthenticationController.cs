﻿using Microsoft.AspNetCore.Mvc;
using OrdersSystem.Api.Auth.Services;
using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.Api.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;

        public AuthenticationController(IJwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost("authenticate/")]
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
