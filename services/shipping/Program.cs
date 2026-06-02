using Microsoft.EntityFrameworkCore;
using ShippingService.Controllers;
using ShippingService.Data;
using ShippingService.Middleware;
using ShippingService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cartEndpoint = builder.Configuration.GetValue<string>("CartEndpoint")
    ?? Environment.GetEnvironmentVariable("CART_ENDPOINT")
    ?? "cart";

var dbHost = builder.Configuration.GetValue<string>("DB_HOST")
    ?? Environment.GetEnvironmentVariable("DB_HOST")
    ?? builder.Configuration.GetValue<string>("MYSQL_HOST")
    ?? Environment.GetEnvironmentVariable("MYSQL_HOST")
    ?? "mysql";

var connectionString = builder.Configuration.GetConnectionString("ShippingDatabase")
    ?? $"server={dbHost};database=cities;user=shipping;password=RoboShop@1";

builder.Services.AddHttpClient<CartHelper>(client => client.Timeout = TimeSpan.FromSeconds(5));
builder.Services.AddSingleton(new CartConfiguration(cartEndpoint));

builder.Services.AddDbContext<ShippingDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 33)), mysqlOptions =>
        mysqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

var app = builder.Build();

app.UseMiddleware<DatacenterTagMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

app.Run();
