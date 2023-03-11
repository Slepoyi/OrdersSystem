namespace OrdersSystem.Api.Middleware
{
    public class AuthorizeAttribute : // auth stuff
    {
        private readonly UserRole _role;
        public AuthorizeAttribute(UserRole role)
        {
            _role = role;
        }
    }
}
