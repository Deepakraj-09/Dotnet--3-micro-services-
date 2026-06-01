using Microsoft.AspNetCore.Mvc;
using RoboShop.Shipping.Models;
using RoboShop.Shipping.Repositories;
using RoboShop.Shipping.Services;

namespace RoboShop.Shipping.Controllers;

/// <summary>
/// REST controller — converted from Java Controller.java (@RestController).
/// Spring @GetMapping / @PostMapping → ASP.NET Core [HttpGet] / [HttpPost].
/// ResponseStatusException → returns IActionResult with appropriate StatusCode.
/// </summary>
[ApiController]
[Route("")]
public class ShippingController : ControllerBase
{
    private static readonly List<byte[]> _bytesGlobal = [];

    private readonly CityRepository _cityRepo;
    private readonly CodeRepository _codeRepo;
    private readonly CartHelper _cartHelper;
    private readonly ILogger<ShippingController> _logger;

    // Home warehouse location (same constants as Java)
    private const double HomeLatitude  = 51.164896;
    private const double HomeLongitude = 7.068792;

    public ShippingController(
        CityRepository cityRepo,
        CodeRepository codeRepo,
        CartHelper cartHelper,
        ILogger<ShippingController> logger)
    {
        _cityRepo  = cityRepo;
        _codeRepo  = codeRepo;
        _cartHelper = cartHelper;
        _logger    = logger;
    }

    // GET /health
    [HttpGet("health")]
    public IActionResult Health() => Ok("OK");

    // GET /count  — returns total city row count
    [HttpGet("count")]
    public async Task<IActionResult> Count()
    {
        var count = await _cityRepo.CountAsync();
        return Ok(count.ToString());
    }

    // GET /codes  — all shipping codes ordered by name
    [HttpGet("codes")]
    public async Task<IActionResult> Codes()
    {
        _logger.LogInformation("all codes");
        var codes = await _codeRepo.FindAllOrderedByNameAsync();
        return Ok(codes);
    }

    // GET /cities/{code}
    [HttpGet("cities/{code}")]
    public async Task<IActionResult> Cities(string code)
    {
        _logger.LogInformation("cities by code {Code}", code);
        var cities = await _cityRepo.FindByCodeAsync(code);
        return Ok(cities);
    }

    // GET /match/{code}/{text}  — autocomplete city search
    [HttpGet("match/{code}/{text}")]
    public async Task<IActionResult> Match(string code, string text)
    {
        _logger.LogInformation("match code {Code} text {Text}", code, text);

        if (text.Length < 3)
            return BadRequest();

        var cities = await _cityRepo.MatchAsync(code, text);

        // Limit to 10 results (matches the Java "dirty hack" comment)
        if (cities.Count > 10)
            cities = cities[..9];

        return Ok(cities);
    }

    // GET /calc/{id}  — calculate shipping cost for a city
    [HttpGet("calc/{id:long}")]
    public async Task<IActionResult> Calculate(long id)
    {
        _logger.LogInformation("Calculation for {Id}", id);

        var city = await _cityRepo.FindByIdAsync(id);
        if (city == null)
            return NotFound("city not found");

        var calc = new Calculator(city);
        long distance = calc.GetDistance(HomeLatitude, HomeLongitude);

        // Match Java: Math.rint(distance * 5) / 100.0
        double cost = Math.Round(distance * 5.0) / 100.0;

        var ship = new Ship(distance, cost);
        _logger.LogInformation("shipping {Ship}", ship);

        return Ok(ship);
    }

    // POST /confirm/{id}  — add shipping info to cart
    [HttpPost("confirm/{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> Confirm(string id, [FromBody] object body)
    {
        _logger.LogInformation("confirm id: {Id}", id);

        string jsonBody = System.Text.Json.JsonSerializer.Serialize(body);
        _logger.LogInformation("body {Body}", jsonBody);

        string cart = await _cartHelper.AddToCartAsync(id, jsonBody);

        if (string.IsNullOrEmpty(cart))
            return NotFound("cart not found");

        return Content(cart, "application/json");
    }

    // GET /memory  — allocate 25MB and hold it (load-test / chaos endpoint)
    [HttpGet("memory")]
    public IActionResult Memory()
    {
        var bytes = new byte[1024 * 1024 * 25];
        Array.Fill(bytes, (byte)8);
        _bytesGlobal.Add(bytes);
        return Ok(_bytesGlobal.Count);
    }

    // GET /free  — release all held memory
    [HttpGet("free")]
    public IActionResult Free()
    {
        _bytesGlobal.Clear();
        return Ok(_bytesGlobal.Count);
    }
}
