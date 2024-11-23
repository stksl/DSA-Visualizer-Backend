using Ocelot.DependencyInjection;
using static Ocelot.Middleware.OcelotMiddlewareExtensions;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Service;
internal class Program
{
    public static void Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddRateLimiter(limiter =>
        {
            limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
            {
                string ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, partition => new FixedWindowRateLimiterOptions()
                {
                    Window = TimeSpan.FromSeconds(10),
                    PermitLimit = 4,
                    QueueLimit = 1,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });
        });
        builder.Services.AddAuthentication().AddCookie(IdentityConstants.BearerScheme);

        builder.Services.AddDbContext<ApplicationDbContext>(options => 
            options.UseNpgsql(builder.Configuration.GetConnectionString("PgsqlConnectionString")));

        builder.Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
        builder.Services.AddOcelot(builder.Configuration);

        WebApplication app = builder.Build();
        app.UseRateLimiter();

        app.Map("/gateway", async appBuilder => 
        {
            await appBuilder.UseOcelot();
        });

        app.MapIdentityApi<User>();
        app.Run();

    }
}