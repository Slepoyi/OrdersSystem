using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrdersSystem.Api.Auth.Middleware;
using OrdersSystem.Api.Auth.Services;
using OrdersSystem.Api.Options;
using OrdersSystem.Data.Access.Context;
using OrdersSystem.Data.Process.DataRefresh;
using OrdersSystem.Data.Process.Options;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Data.Process.Validation;
using OrdersSystem.Domain.Helper;
using OrdersSystem.Domain.Time;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering system", Version = "v1" });
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Ordering system.xml"));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DesktopConnection"));
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Section));
builder.Services.Configure<FakerOptions>(builder.Configuration.GetSection(FakerOptions.Section));

builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationContext>();

builder.Services.AddTransient<IClock, Clock>();
builder.Services.AddTransient<ICryptoProvider, Md5HashProvider>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<IOrderFlowManager, OrderFlowManager>();
builder.Services.AddTransient<IOrderValidator, OrderValidator>();
builder.Services.AddTransient<IDbSetsRefresher, DbSetsRefresher>();
builder.Services.AddTransient<IDataGenerator, DataGenerator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

app.Run();

public partial class Program { }