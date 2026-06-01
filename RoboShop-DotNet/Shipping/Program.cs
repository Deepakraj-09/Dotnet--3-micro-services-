using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using RoboShop.Shipping.Controllers;
using RoboShop.Shipping.Infrastructure;
using RoboShop.Shipping.Repositories;
using RoboShop.Shipping.Services;

// ─── Serilog (replaces SLF4J / Logback) ───────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// ─── Database (replaces JpaConfig.java + Spring Data JPA auto-config) ─────────
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "mysql";
var connStr = $"Server={dbHost};Database=cities;User=shipping;Password=RoboShop@1;SslMode=None;AllowPublicKeyRetrieval=true;";

// Polly retry on transient DB failures — replaces Java RetryableDataSource + @Retryable
builder.Services.AddDbContext<ShippingDbContext>(opts =>
    opts.UseMySql(connStr, ServerVersion.AutoDetect(connStr),
        mySqlOpts => mySqlOpts.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// ─── Repositories ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<CityRepository>();
builder.Services.AddScoped<CodeRepository>();

// ─── Cart HTTP client (replaces Apache HttpClient in CartHelper.java) ──────────
//     Polly retry: 10 attempts, exponential backoff up to 30 s
//     (mirrors Java @Retryable(maxAttempts=10, backoff=@Backoff(multiplier=2.3, maxDelay=30000)))
var cartEndpoint = Environment.GetEnvironmentVariable("CART_ENDPOINT") ?? "cart";

builder.Services.AddHttpClient<CartHelper>(client =>
{
    client.BaseAddress = new Uri($"http://{cartEndpoint}/");
    client.Timeout = TimeSpan.FromSeconds(5); // matches Java HttpConnectionParams.setConnectionTimeout(5000)
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(10, retryAttempt =>
        TimeSpan.FromMilliseconds(Math.Min(300 * Math.Pow(2.3, retryAttempt - 1), 30_000))));

// ─── MVC / API ─────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

var app = builder.Build();

// ─── DatacenterMiddleware (replaces InstanaDatacenterTagInterceptor) ───────────
app.UseMiddleware<DatacenterMiddleware>();

app.MapControllers();

app.Run();
