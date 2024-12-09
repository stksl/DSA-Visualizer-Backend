using System.Net;
using System.Net.WebSockets;
using Dsa.Service;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment()) 
{
    builder.Services.AddSwaggerGen();
    builder.Services.AddEndpointsApiExplorer();
}
builder.Services.AddSingleton<IAddressProvider, GatewayAddressProvider>(provider => 
    new GatewayAddressProvider(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5091)));
builder.Services.AddSignalR();
builder.Services.AddControllers();
var app = builder.Build();

app.UseIpConstrainer();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHub<DsaHub>("dsahub");
app.Run();