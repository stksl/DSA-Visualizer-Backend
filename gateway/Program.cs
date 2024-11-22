using Ocelot.DependencyInjection;
using static Ocelot.Middleware.OcelotMiddlewareExtensions;
using System.Threading.RateLimiting;

namespace BackendGateway;
internal class Program
{
    public static async Task Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddRateLimiter(limiter =>
        {
            limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
            {
                string ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, partition => new FixedWindowRateLimiterOptions()
                {
                    Window = TimeSpan.FromSeconds(1),
                    PermitLimit = 4,
                    QueueLimit = 1,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });
        });

        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
        builder.Services.AddOcelot(builder.Configuration);

        WebApplication app = builder.Build();
        app.UseRateLimiter();
        await app.UseOcelot();

        app.Run();

    }
}