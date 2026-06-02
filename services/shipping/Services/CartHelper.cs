using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Services
{
    public sealed class CartConfiguration
    {
        public string CartUrl { get; }

        public CartConfiguration(string cartEndpoint)
        {
            CartUrl = $"http://{cartEndpoint}/shipping/";
        }
    }

    public class CartHelper
    {
        private readonly HttpClient _httpClient;
        private readonly CartConfiguration _configuration;

        public CartHelper(HttpClient httpClient, CartConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> AddToCartAsync(string id, string data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _configuration.CartUrl + id)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
