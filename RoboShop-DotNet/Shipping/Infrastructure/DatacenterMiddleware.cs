namespace RoboShop.Shipping.Infrastructure;

/// <summary>
/// ASP.NET Core middleware that tags each request with a random datacenter.
/// Converted from Java InstanaDatacenterTagInterceptor (HandlerInterceptorAdapter).
///
/// The original used Instana's SpanSupport.annotate() for tracing.
/// Here the tag is written as a response header (X-Datacenter) and an
/// Activity tag so it works with OpenTelemetry or any tracing backend.
/// </summary>
public class DatacenterMiddleware
{
    private static readonly string[] DataCenters =
    [
        "asia-northeast2",
        "asia-south1",
        "europe-west3",
        "us-east1",
        "us-west1"
    ];

    private static readonly Random Rng = new();
    private readonly RequestDelegate _next;

    public DatacenterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string dc = DataCenters[Rng.Next(DataCenters.Length)];

        // Attach to current Activity for distributed tracing (OpenTelemetry compatible)
        System.Diagnostics.Activity.Current?.SetTag("datacenter", dc);

        // Also expose as a response header for observability / debugging
        context.Response.Headers["X-Datacenter"] = dc;

        await _next(context);
    }
}
