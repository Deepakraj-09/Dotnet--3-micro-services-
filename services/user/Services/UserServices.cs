using MongoDB.Driver;
using StackExchange.Redis;
using UserService.Models;

namespace UserService.Services
{
    public sealed class MongoConfiguration
    {
        public string MongoUrl { get; }

        public MongoConfiguration(string mongoUrl)
        {
            MongoUrl = mongoUrl;
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
                _db = client.GetDatabase("users");
                IsConnected = true;
                _logger.LogInformation("MongoDB connected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MongoDB");
                IsConnected = false;
            }
        }

        public IMongoCollection<User> GetUsersCollection()
        {
            return _db.GetCollection<User>("users");
        }

        public IMongoCollection<Order> GetOrdersCollection()
        {
            return _db.GetCollection<Order>("orders");
        }
    }

    public class RedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ILogger<RedisService> _logger;

        public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger)
        {
            _redis = redis;
            _db = redis.GetDatabase();
            _logger = logger;
        }

        public async Task<long> IncrementCounterAsync(string key)
        {
            return await _db.StringIncrementAsync(key);
        }
    }
}
