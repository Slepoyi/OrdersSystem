using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrdersSystem.Data.Access.Context;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Data.Process.Validation;
using OrdersSystem.Domain.Time;

namespace OrdersSystem.IntegrationTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer("Server=DESKTOP-B0H7875\\SQLEXPRESS; Database=OrdDb; Trusted_Connection=True; TrustServerCertificate=Yes;");
                //options.UseSqlServer("Server=DESKTOP-AOPEUHQ\\SQLEXPRESS; Database=OrdDb; Trusted_Connection=True; TrustServerCertificate=Yes;");
            });
            services.AddTransient<IClock, Clock>();
            services.AddTransient<IOrderValidator, OrderValidator>();
            services.AddTransient<IOrderFlowManager, OrderFlowManager>();
            services.AddTransient<WebApplicationFactory<Program>>();
        }
    }
}
