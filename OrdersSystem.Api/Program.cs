using Microsoft.EntityFrameworkCore;
using OrdersSystem.Api.Auth.Middleware;
using OrdersSystem.Api.Auth.Services;
using OrdersSystem.Api.Options;
using OrdersSystem.Data.Access.Context;
using OrdersSystem.Data.Process.Services;
using OrdersSystem.Data.Process.Validation;
using OrdersSystem.Data.Refresh;
using OrdersSystem.Domain.Helper;
using OrdersSystem.Domain.Time;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DesktopConnection"));
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Section));

builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationContext>();

builder.Services.AddTransient<IClock, Clock>();
builder.Services.AddTransient<ICryptoProvider, Md5HashProvider>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<IOrderFlowManager, OrderFlowManager>();
builder.Services.AddTransient<IOrderValidator, OrderValidator>();
builder.Services.AddTransient<IRefreshDbSets, RefreshDbSets>();

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
