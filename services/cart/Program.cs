using StackExchange.Redis;
using CartService.Controllers;
using CartService.Middleware;
using CartService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redisHost = builder.Configuration.GetValue<string>("RedisHost")
    ?? Environment.GetEnvironmentVariable("REDIS_HOST")
    ?? "redis";

var catalogueHost = builder.Configuration.GetValue<string>("CatalogueHost")
    ?? Environment.GetEnvironmentVariable("CATALOGUE_HOST")
    ?? "catalogue";

var cataloguePort = builder.Configuration.GetValue<string>("CataloguePort")
    ?? Environment.GetEnvironmentVariable("CATALOGUE_PORT")
    ?? "8080";

var redisConnectionString = $"{redisHost}:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddSingleton<RedisService>();
builder.Services.AddHttpClient<CatalogueService>(client => client.Timeout = TimeSpan.FromSeconds(5));
builder.Services.AddSingleton(new CatalogueConfiguration(catalogueHost, cataloguePort));

var app = builder.Build();

app.UseMiddleware<DatacenterTagMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithExposedHeaders("*"));

app.UseRouting();
app.MapControllers();

app.Run();
