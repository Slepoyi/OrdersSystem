using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrdersSystem.Domain.Models.Auth;

namespace OrdersSystem.Api.Auth.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private string[] _roles;
        public AuthorizeAttribute(params string[] roles) : base()
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            var user = context.HttpContext.Items["User"] as User;
            if (user is null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }

            if (!_roles.Contains(user.Role))
            {
                context.Result = new JsonResult(new { message = "Insufficient rights" }) { StatusCode = StatusCodes.Status403Forbidden };
                return;
            }
        }
    }
}
