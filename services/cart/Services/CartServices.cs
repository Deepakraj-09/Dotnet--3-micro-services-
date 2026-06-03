using StackExchange.Redis;
using System.Text.Json;
using CartService.Models;

namespace CartService.Services
{
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

        public bool IsConnected => _redis.IsConnected;

        public async Task<string?> GetCartAsync(string id)
        {
            var value = await _db.StringGetAsync(id);
            return value.IsNull ? null : value.ToString();
        }

        public async Task<bool> SetCartAsync(string id, Cart cart)
        {
            var json = JsonSerializer.Serialize(cart);
            return await _db.StringSetAsync(id, json);
        }

        public async Task<bool> DeleteCartAsync(string id)
        {
            return await _db.KeyDeleteAsync(id) > 0;
        }
    }

    public sealed class CatalogueConfiguration
    {
        public string CatalogueUrl { get; }

        public CatalogueConfiguration(string host, string port)
        {
            CatalogueUrl = $"http://{host}:{port}";
        }
    }

    public class CatalogueService
    {
        private readonly HttpClient _httpClient;
        private readonly CatalogueConfiguration _config;
        private readonly ILogger<CatalogueService> _logger;

        public CatalogueService(HttpClient httpClient, CatalogueConfiguration config, ILogger<CatalogueService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<Product?> GetProductAsync(string sku)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_config.CatalogueUrl}/product/{sku}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Product>(content);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {Sku}", sku);
                return null;
            }
        }
    }
}
