using Ocelot.DependencyInjection;
using static Ocelot.Middleware.OcelotMiddlewareExtensions;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace Gateway.Service;
internal class Program
{
    public static void Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        if (builder.Environment.IsDevelopment()) 
        {
            builder.Services.AddSwaggerGen();
            builder.Services.AddEndpointsApiExplorer();
        }
        builder.Services.AddRateLimiter(limiter =>
        {
            limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
            {
                string ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, partition => new FixedWindowRateLimiterOptions()
                {
                    Window = TimeSpan.FromSeconds(1),
                    PermitLimit = 5,
                    QueueLimit = 1,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });
        });
        
        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddCookie(IdentityConstants.ApplicationScheme);

        builder.Services.AddDbContext<ApplicationDbContext>(options => 
            options.UseNpgsql(builder.Configuration.GetConnectionString("PgsqlConnectionString")));

        builder.Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
        builder.Services.AddOcelot(builder.Configuration);

        WebApplication app = builder.Build();
        app.UseRateLimiter();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.Map("/gateway", [Authorize] async (IApplicationBuilder appBuilder) => 
        {
            await appBuilder.UseOcelot();
        });
        app.MapIdentityApi<User>();

        app.Run();

    }
}