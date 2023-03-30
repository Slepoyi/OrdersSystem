using Microsoft.Extensions.Configuration;
using OrdersSystem.Api.Options;

namespace OrdersSystem.UnitTests.Options
{
    public class UnitTests
    {
        private readonly ConfigBinder _configBinder;
        private readonly Dictionary<string, string> _inMemorySettings 
            = new()
            {
                {"JwtOptions", "{\"ValidateIssuer\": 1, \"ValidateAudience\": 1, \"ValidateLifetime\": 1, \"ValidateIssuerSigningKey\": 1, \"ValidAudience\": \"http://localhost:4200\", \"ValidIssuer\": \"http://localhost:5000\", \"Secret\": \"JwtAuthenticationHighlySecuredPasswordIs42069\"}"},
            };
        
        public UnitTests()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_inMemorySettings)
                .Build();
            _configBinder = new ConfigBinder(configuration);
        }

        [Fact]
        public void BindConfiguration_ReturnsTrue_WhenJwtOptionsFieldsMappedCorrectly()
        {
            var jwtOptions = new JwtOptions();

            jwtOptions = _configBinder.BindConfiguration<JwtOptions>(JwtOptions.Section) as JwtOptions;

            Assert.NotNull(jwtOptions);
            Assert.Equal("JwtAuthenticationHighlySecuredPasswordIs42069", jwtOptions.Secret);
            Assert.Equal("ValidIssuer", "http://localhost:4200");
            Assert.True(jwtOptions.ValidateIssuerSigningKey);
            Assert.True(jwtOptions.ValidateIssuer);
        }
    }
}
