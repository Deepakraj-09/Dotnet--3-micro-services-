namespace RoboShop.Shipping.Services;

/// <summary>
/// HTTP helper to add shipping details to the cart service.
/// Converted from Java CartHelper.java — replaces Apache HttpClient with .NET HttpClient.
/// HttpClient is injected (registered as a named/typed client in Program.cs with Polly retry).
/// </summary>
public class CartHelper
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CartHelper> _logger;

    public CartHelper(HttpClient httpClient, ILogger<CartHelper> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// POST shipping data to the cart service for the given cart ID.
    /// Returns the response body, or empty string on failure (matches Java behaviour).
    /// </summary>
    public async Task<string> AddToCartAsync(string id, string jsonBody)
    {
        _logger.LogInformation("Add shipping to cart {Id}", id);

        try
        {
            var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"shipping/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            _logger.LogWarning("Failed with status code {StatusCode}", (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "HTTP client exception");
        }

        // Empty string on error — same contract as the Java version
        return string.Empty;
    }
}
