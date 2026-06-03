using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using CatalogueService.Models;
using CatalogueService.Services;

namespace CatalogueService.Controllers
{
    [ApiController]
    [Route("")]
    public class CatalogueController : ControllerBase
    {
        private readonly MongoDbService _mongoService;
        private readonly CatalogueConfiguration _config;
        private readonly ILogger<CatalogueController> _logger;

        public CatalogueController(MongoDbService mongoService, CatalogueConfiguration config, ILogger<CatalogueController> logger)
        {
            _mongoService = mongoService;
            _config = config;
            _logger = logger;
        }

        [HttpGet("health")]
        public HealthStatus Health()
        {
            return new HealthStatus { App = "OK", Mongo = _mongoService.IsConnected };
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            if (!_mongoService.IsConnected)
            {
                _logger.LogError("database not available");
                return StatusCode(500, "database not available");
            }

            try
            {
                var collection = _mongoService.GetProductsCollection();
                var products = await collection.Find(_ => true).ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("product/{sku}")]
        public async Task<IActionResult> GetProductBySku(string sku)
        {
            if (!_mongoService.IsConnected)
            {
                _logger.LogError("database not available");
                return StatusCode(500, "database not available");
            }

            try
            {
                var delay = _config.GoSlow;
                if (delay > 0)
                {
                    await Task.Delay(delay);
                }

                var collection = _mongoService.GetProductsCollection();
                var product = await collection.Find(p => p.Sku == sku).FirstOrDefaultAsync();

                if (product == null)
                {
                    return NotFound("SKU not found");
                }

                _logger.LogInformation("product {Product}", product);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("products/{cat}")]
        public async Task<IActionResult> GetProductsByCategory(string cat)
        {
            if (!_mongoService.IsConnected)
            {
                _logger.LogError("database not available");
                return StatusCode(500, "database not available");
            }

            try
            {
                var collection = _mongoService.GetProductsCollection();
                var products = await collection
                    .Find(p => p.Categories!.Contains(cat))
                    .SortBy(p => p.Name)
                    .ToListAsync();

                if (!products.Any())
                {
                    return NotFound($"No products for {cat}");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            if (!_mongoService.IsConnected)
            {
                _logger.LogError("database not available");
                return StatusCode(500, "database not available");
            }

            try
            {
                var collection = _mongoService.GetProductsCollection();
                var categories = await collection.Distinct<string>("categories", FilterDefinition<Product>.Empty).ToListAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search/{text}")]
        public async Task<IActionResult> Search(string text)
        {
            if (!_mongoService.IsConnected)
            {
                _logger.LogError("database not available");
                return StatusCode(500, "database not available");
            }

            try
            {
                var collection = _mongoService.GetProductsCollection();
                var filter = Builders<Product>.Filter.Or(
                    Builders<Product>.Filter.Regex(p => p.Name!, text),
                    Builders<Product>.Filter.Regex(p => p.Description!, text)
                );
                var hits = await collection.Find(filter).ToListAsync();
                return Ok(hits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
