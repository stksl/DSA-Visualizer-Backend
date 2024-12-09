using System.Net;
namespace Dsa.Service;
internal sealed class GatewayAddressProvider : IAddressProvider 
{
    private readonly IPEndPoint _gatewayAddr;
    public GatewayAddressProvider(IPEndPoint gatewayAddress)
    {
        _gatewayAddr = gatewayAddress;
    }

    public IPEndPoint GetAddress() => _gatewayAddr;
} 