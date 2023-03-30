using Microsoft.Extensions.Configuration;

namespace OrdersSystem.UnitTests.Options
{
    public class ConfigBinder
    {
        private readonly IConfiguration _configuration;
        public ConfigBinder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object? BindConfiguration<T>(string sectionName)
        {
            if (sectionName == string.Empty)
                throw new ArgumentException(null, nameof(sectionName));

            var instance = Activator.CreateInstance(typeof(T));
            var section = _configuration.GetSection(sectionName);
            section.Bind(instance);
            return instance;
        }
    }
}
