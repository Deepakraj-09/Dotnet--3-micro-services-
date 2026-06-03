using MongoDB.Driver;
using StackExchange.Redis;
using UserService.Controllers;
using UserService.Middleware;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoUrl = builder.Configuration.GetValue<string>("MongoUrl")
    ?? Environment.GetEnvironmentVariable("MONGO_URL")
    ?? "mongodb://mongodb:27017/users";

var redisHost = builder.Configuration.GetValue<string>("RedisHost")
    ?? Environment.GetEnvironmentVariable("REDIS_HOST")
    ?? "redis";

var redisConnectionString = $"{redisHost}:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<RedisService>();
builder.Services.AddSingleton(new MongoConfiguration(mongoUrl));

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
