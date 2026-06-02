using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ShippingService.Data;
using ShippingService.Models;
using ShippingService.Services;

namespace ShippingService.Controllers
{
    [ApiController]
    [Route("")]
    public class ShippingController : ControllerBase
    {
        private static readonly List<byte[]> BytesGlobal = new();
        private static readonly string[] DataCenters = new[]
        {
            "asia-northeast2",
            "asia-south1",
            "europe-west3",
            "us-east1",
            "us-west1"
        };

        private readonly ShippingDbContext _context;
        private readonly CartHelper _cartHelper;
        private readonly ILogger<ShippingController> _logger;
        private readonly Random _random = new();

        public ShippingController(ShippingDbContext context, CartHelper cartHelper, ILogger<ShippingController> logger)
        {
            _context = context;
            _cartHelper = cartHelper;
            _logger = logger;
        }

        [HttpGet("memory")]
        public int Memory()
        {
            var bytes = new byte[1024 * 1024 * 25];
            Array.Fill(bytes, (byte)8);
            BytesGlobal.Add(bytes);
            return BytesGlobal.Count;
        }

        [HttpGet("free")]
        public int Free()
        {
            BytesGlobal.Clear();
            return BytesGlobal.Count;
        }

        [HttpGet("health")]
        public string Health() => "OK";

        [HttpGet("count")]
        public async Task<string> Count()
        {
            var count = await _context.Cities.LongCountAsync();
            return count.ToString();
        }

        [HttpGet("codes")]
        public async Task<IEnumerable<Code>> Codes()
        {
            _logger.LogInformation("all codes");
            return await _context.Codes.OrderBy(c => c.Name).ToListAsync();
        }

        [HttpGet("cities/{code}")]
        public async Task<IEnumerable<City>> Cities(string code)
        {
            _logger.LogInformation("cities by code {Code}", code);
            return await _context.Cities.Where(c => c.Code == code).ToListAsync();
        }

        [HttpGet("match/{code}/{text}")]
        public async Task<IActionResult> Match(string code, string text)
        {
            _logger.LogInformation("match code {Code} text {Text}", code, text);

            if (text.Length < 3)
            {
                return BadRequest();
            }

            var cities = await _context.Cities
                .Where(c => c.Code == code && c.CityName != null && c.CityName.StartsWith(text))
                .OrderBy(c => c.CityName)
                .Take(10)
                .ToListAsync();

            return Ok(cities);
        }

        [HttpGet("calc/{id}")]
        public async Task<IActionResult> Calc(long id)
        {
            const double homeLatitude = 51.164896;
            const double homeLongitude = 7.068792;

            _logger.LogInformation("Calculation for {Id}", id);

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound("city not found");
            }

            var calc = new Calculator(city);
            var distance = calc.GetDistance(homeLatitude, homeLongitude);
            var cost = Math.Round(distance * 5.0) / 100.0;
            var ship = new Ship(distance, cost);

            _logger.LogInformation("shipping {Ship}", ship);
            return Ok(ship);
        }

        [HttpPost("confirm/{id}")]
        public async Task<IActionResult> Confirm(string id, [FromBody] JsonElement body)
        {
            _logger.LogInformation("confirm id: {Id}", id);
            _logger.LogInformation("body {Body}", body.ToString());

            var result = await _cartHelper.AddToCartAsync(id, body.ToString());
            if (string.IsNullOrWhiteSpace(result))
            {
                return NotFound("cart not found");
            }

            return Ok(result);
        }
    }
}
