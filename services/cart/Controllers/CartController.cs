using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CartService.Models;
using CartService.Services;

namespace CartService.Controllers
{
    [ApiController]
    [Route("")]
    public class CartController : ControllerBase
    {
        private static int _itemsAdded = 0;

        private readonly RedisService _redisService;
        private readonly CatalogueService _catalogueService;
        private readonly ILogger<CartController> _logger;

        public CartController(RedisService redisService, CatalogueService catalogueService, ILogger<CartController> logger)
        {
            _redisService = redisService;
            _catalogueService = catalogueService;
            _logger = logger;
        }

        [HttpGet("health")]
        public HealthStatus Health()
        {
            return new HealthStatus { App = "OK", Redis = _redisService.IsConnected };
        }

        [HttpGet("metrics")]
        public string Metrics()
        {
            Response.ContentType = "text/plain";
            return $"# HELP items_added running count of items added to cart\n# TYPE items_added counter\nitems_added {_itemsAdded}";
        }

        [HttpGet("cart/{id}")]
        public async Task<IActionResult> GetCart(string id)
        {
            try
            {
                var data = await _redisService.GetCartAsync(id);
                if (string.IsNullOrEmpty(data))
                {
                    return NotFound("cart not found");
                }
                Response.ContentType = "application/json";
                return Content(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("cart/{id}")]
        public async Task<IActionResult> DeleteCart(string id)
        {
            try
            {
                var result = await _redisService.DeleteCartAsync(id);
                return result ? Ok("OK") : NotFound("cart not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("rename/{from}/{to}")]
        public async Task<IActionResult> RenameCart(string from, string to)
        {
            try
            {
                var data = await _redisService.GetCartAsync(from);
                if (string.IsNullOrEmpty(data))
                {
                    return NotFound("cart not found");
                }

                var cart = JsonSerializer.Deserialize<Cart>(data);
                if (cart != null)
                {
                    await _redisService.SetCartAsync(to, cart);
                }
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renaming cart");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("add/{id}/{sku}/{qty}")]
        public async Task<IActionResult> AddToCart(string id, string sku, string qty)
        {
            try
            {
                if (!int.TryParse(qty, out var quantity) || quantity < 1)
                {
                    return BadRequest("quantity must be a number greater than zero");
                }

                var product = await _catalogueService.GetProductAsync(sku);
                if (product == null)
                {
                    return NotFound("product not found");
                }

                if (product.Instock == 0)
                {
                    return NotFound("out of stock");
                }

                var cartData = await _redisService.GetCartAsync(id);
                Cart cart;
                if (string.IsNullOrEmpty(cartData))
                {
                    cart = new Cart { Total = 0, Tax = 0, Items = new() };
                }
                else
                {
                    cart = JsonSerializer.Deserialize<Cart>(cartData) ?? new Cart();
                }

                var item = new CartItem
                {
                    Qty = quantity,
                    Sku = sku,
                    Name = product.Name,
                    Price = product.Price,
                    Subtotal = quantity * product.Price
                };

                var existing = cart.Items.FirstOrDefault(i => i.Sku == sku);
                if (existing != null)
                {
                    existing.Qty += quantity;
                    existing.Subtotal = existing.Qty * existing.Price;
                }
                else
                {
                    cart.Items.Add(item);
                }

                cart.Total = cart.Items.Sum(i => i.Subtotal);
                cart.Tax = Math.Round(cart.Total * 0.15, 2);

                await _redisService.SetCartAsync(id, cart);
                _itemsAdded += quantity;

                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
