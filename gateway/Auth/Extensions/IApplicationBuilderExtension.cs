namespace Gateway.Service;
public static class IApplicationBuilderExtension 
{
    public static IApplicationBuilder UseAuthHandler(this IApplicationBuilder app) 
    {
        app.UseMiddleware<AuthHandlerMiddleware>();
        return app;
    }
}