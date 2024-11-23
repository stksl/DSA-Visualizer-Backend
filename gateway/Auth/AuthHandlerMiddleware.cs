public sealed class AuthHandlerMiddleware 
{
    private readonly RequestDelegate _next;
    public AuthHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext ctx) 
    {
        await _next(ctx);
    }
}