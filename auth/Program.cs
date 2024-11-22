using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("PgsqlConnectionString")));

        builder.Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.MapIdentityApi<User>();
        app.Run();
    }
}