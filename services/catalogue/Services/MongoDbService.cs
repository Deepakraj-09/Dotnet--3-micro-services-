using MongoDB.Driver;
using CatalogueService.Models;

namespace CatalogueService.Services
{
    public sealed class MongoConfiguration
    {
        public string MongoUrl { get; }

        public MongoConfiguration(string mongoUrl)
        {
            MongoUrl = mongoUrl;
        }
    }

    public sealed class CatalogueConfiguration
    {
        public int GoSlow { get; }

        public CatalogueConfiguration(int goSlow)
        {
            GoSlow = goSlow;
        }
    }

    public class MongoDbService
    {
        private readonly IMongoDatabase _db;
        private readonly ILogger<MongoDbService> _logger;
        public bool IsConnected { get; private set; }

        public MongoDbService(MongoConfiguration config, ILogger<MongoDbService> logger)
        {
            _logger = logger;
            try
            {
                var client = new MongoClient(config.MongoUrl);
                _db = client.GetDatabase("catalogue");
                IsConnected = true;
                _logger.LogInformation("MongoDB connected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MongoDB");
                IsConnected = false;
            }
        }

        public IMongoCollection<Product> GetProductsCollection()
        {
            return _db.GetCollection<Product>("products");
        }
    }
}
