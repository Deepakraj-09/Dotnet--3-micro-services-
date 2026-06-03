namespace CatalogueService.Middleware
{
    public class DatacenterTagMiddleware
    {
        private static readonly string[] DataCenters = new[]
        {
            "asia-northeast2",
            "asia-south1",
            "europe-west3",
            "us-east1",
            "us-west1"
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<DatacenterTagMiddleware> _logger;
        private readonly Random _random = new();

        public DatacenterTagMiddleware(RequestDelegate next, ILogger<DatacenterTagMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var datacenter = DataCenters[_random.Next(DataCenters.Length)];
            _logger.LogInformation("datacenter={DataCenter}", datacenter);
            context.Items["Datacenter"] = datacenter;
            await _next(context);
        }
    }
}
