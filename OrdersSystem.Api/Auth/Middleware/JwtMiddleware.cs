using OrdersSystem.Api.Auth.Services;

namespace OrdersSystem.Api.Auth.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtService jwtService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var username = jwtService.ValidateTokenAndExtractUsername(token);
            if (username is null)
                return;

            context.Items["User"] = await userService.GetByUsernameAsync(username);

            await _next(context);
        }
    }
}
