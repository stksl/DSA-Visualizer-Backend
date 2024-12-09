namespace Dsa.Service;
public static class IApplicationBuilderExtention 
{
    public static IApplicationBuilder UseIpConstrainer(this IApplicationBuilder builder) =>
        builder.UseMiddleware<IPConstrainerMiddleware>();
}