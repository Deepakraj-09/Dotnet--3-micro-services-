using MongoDB.Driver;
using CatalogueService.Controllers;
using CatalogueService.Middleware;
using CatalogueService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoUrl = builder.Configuration.GetValue<string>("MongoUrl")
    ?? Environment.GetEnvironmentVariable("MONGO_URL")
    ?? "mongodb://mongodb:27017/catalogue";

var goSlow = builder.Configuration.GetValue<int>("GoSlow")
    ?? (Environment.GetEnvironmentVariable("GO_SLOW") != null ? int.Parse(Environment.GetEnvironmentVariable("GO_SLOW")!) : 0);

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton(new MongoConfiguration(mongoUrl));
builder.Services.AddSingleton(new CatalogueConfiguration(goSlow));

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
