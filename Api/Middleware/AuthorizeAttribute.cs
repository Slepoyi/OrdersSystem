namespace Api.Middleware
{
    public class AuthorizeAttribute
    {
        private readonly UserRole _role;
        public AuthorizeAttribute(UserRole role)
        {
            _role = role;
        }
    }
}
