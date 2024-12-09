using System.Net;
using Dsa.Service;
namespace Dsa.Service;
public sealed class IPConstrainerMiddleware 
{
    private readonly RequestDelegate _next;
    private readonly IAddressProvider _addrProvider;
    public IPConstrainerMiddleware(RequestDelegate next, IAddressProvider addressProvider)
    {
        _next = next;
        _addrProvider = addressProvider;
    }
    public Task Invoke(HttpContext ctx) 
    {
        /*if (ctx.Request.Host.Value != _addrProvider.GetAddress().ToString()) 
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        } */
        return _next(ctx);
    }
}